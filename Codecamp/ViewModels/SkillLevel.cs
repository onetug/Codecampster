using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public enum LevelsOfSkill
    {
        AllSkillLevels = 1,
        SomePriorKnowledgeRequired = 2,
        DeepDive = 3
    }

    public class SkillLevel
    {
        public static string AllSkillLevels = "All Skill Levels";
        public static string SomePriorKnowledgeRequired = "Some Prior Knowldge Required";
        public static string DeepDive = "Deep Dive";

        public static IList<SkillLevel> GetSkillLevels()
        {
            var skillLevels = new List<SkillLevel>();

            skillLevels.Add(new SkillLevel { SkillLevelId = (int)LevelsOfSkill.AllSkillLevels, Description = AllSkillLevels });
            skillLevels.Add(new SkillLevel { SkillLevelId = (int)LevelsOfSkill.SomePriorKnowledgeRequired, Description = SomePriorKnowledgeRequired });
            skillLevels.Add(new SkillLevel { SkillLevelId = (int)LevelsOfSkill.DeepDive, Description = DeepDive });

            return skillLevels;
        }

        public static string GetSkillLevelDescription(int skillLevel)
        {
            switch (skillLevel)
            {
                case (int)LevelsOfSkill.AllSkillLevels:
                    return AllSkillLevels;
                case (int)LevelsOfSkill.SomePriorKnowledgeRequired:
                    return SomePriorKnowledgeRequired;
                case (int)LevelsOfSkill.DeepDive:
                    return DeepDive;
                default:
                    return "Not specified";
            }
        }

        public int SkillLevelId { get; set; }
        public string Description { get; set; }
    }
}
