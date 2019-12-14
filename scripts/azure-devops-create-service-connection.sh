#!/bin/bash
# 
# Purpose: this script creates a service connection in Azure DevOps
# 
# Pre-Requisites: Install the Azure CLI and the Azure CLI DevOps extension. You will also need JQ installed
# For more information, see https://gaunacode.com/creating-secure-service-connections-in-azure-devops

az devops configure --defaults organization=https://dev.azure.com/nebbiademo project=OrlandoCodeCamp

teamProject='OrlandoCodeCamp'
name='codecampster-nebbia-partner'
subscriptionId=$(az account show | jq -r '.id')
subscriptionName=$(az account show | jq -r '.name')
resourceGroupName1='rg-codecamp-dev-001'
resourceGroupName2='rg-codecamp-core-001'

echo 'creating service principal in Azure AD'

response=$(az ad sp create-for-rbac -n $name --role contributor \
    --scopes /subscriptions/$subscriptionId/resourceGroups/$resourceGroupName1 \
             /subscriptions/$subscriptionId/resourceGroups/$resourceGroupName2)

echo 'parsing response'
appId=$(echo $response | jq -r '.appId')
tenantId=$(echo $response | jq -r '.tenant')
export AZURE_DEVOPS_EXT_AZURE_RM_SERVICE_PRINCIPAL_KEY=$(echo $response | jq -r '.password')

echo 'creating service endpoint'
az devops service-endpoint azurerm create --azure-rm-service-principal-id "$appId" \
                                          --azure-rm-subscription-id "$subscriptionId" \
                                          --azure-rm-subscription-name "$subscriptionName" \
                                          --azure-rm-tenant-id "$tenantId" \
                                          --name "$name" \
                                          --project "$teamProject"