// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function setSpeakerImage(img, id) {
    let url = location.origin + "/api/speakers/" + id + "/image";
    fetch(url)
        .then(function (response) {
            if (response.ok) {
                return response.blob();
            }
        })
        .then(function (blob) {
            const objUrl = URL.createObjectURL(blob);
            img.src = objUrl;
        })
        .catch(function (err) {
            console.log(err);
        });
}

function setSponsorImage(img, id) {
    let url = location.origin + "/api/sponsors/" + id + "/image";
    fetch(url)
        .then(function (response) {
            if (response.ok) {
                return response.blob();
            }
        })
        .then(function (blob) {
            const objUrl = URL.createObjectURL(blob);
            img.src = objUrl;
        })
        .catch(function (err) {
            console.log(err);
        });
}