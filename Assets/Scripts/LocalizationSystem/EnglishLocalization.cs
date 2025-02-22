using System.Collections.Generic;
using UnityEngine;

namespace LocalizationSystem
{

    public class EnglishLocalization : ILocalization
    {
        public string Language => "English";
        private Dictionary<string, string> localizedTexts;

        public EnglishLocalization()
        {
            LoadLocalizationData();
        }

        public void LoadLocalizationData()
        {
            localizedTexts = new Dictionary<string, string>
            {
                // Welcome Screen
                { "welcome_title", "Welcome to Noroland" },
                { "app_description", "Noroland" },
                { "get_ready", "Getting ready to make learning fun" },
                { "who_uses_app", "Who will be using the app?" },
                { "child_option", "Child" },
                { "parent_option", "Parent" },

                // Subscription Screen
                { "today_investment", "Invest today (NAME)" },
                { "save_percent", "Save 45% with annual subscription" },

                // Plan Options
                { "one_month", "1 Month" },
                { "three_months", "3 Months" },
                { "one_year", "1 Year" },
                { "monthly_price_1m", "59,000 Tomans" },
                { "monthly_price_3m", "40,000 Tomans" },
                { "monthly_price_1y", "32,500 Tomans" },
                { "total_price_1m", "59,000 Tomans" },
                { "total_price_3m", "120,000 Tomans" },
                { "total_price_1y", "390,000 Tomans" },
                { "most_popular", "Most Popular" },
                { "best_offer", "Best Offer" },

                // Onboarding Steps
                { "child_personalization", "Child Profile Customization" },
                { "select_age", "Select Appropriate Age" },
                { "start_learning", "Begin Learning" },
                { "continue_progress", "Continue Progress" },
                // Graph 
                { "memory", "Memory" },
                { "language", "Language" },
                { "life_skills", "Life Skills" },
                { "math_and_logic", "Math and logic" },
                { "attention", "Attention" },

                // Buttons
                { "view_all_plans", "View All Plans" },
                { "start_3_day_trial", "Start 3-Day Free Trial" },
                { "next_button", "Next" },

                // Parent info screen
                { "parent_info_title", "Information for Parents and Guardians" },
                { "info_summary", "To all informed parents and guardians, welcome to Noroland!" },
                { "count_prompt", "Count: One, Two, Three" },
                { "progress_percent", "0%" },
                { "personalization_message", "Personalizing the learning path" },

                // Child info questions
                { "child_name_question", "What is your child's name?" },
                { "child_age_question", "How old is (NAME)?" },
                { "help_areas_question", "How can Noroland help?" },

                // Age options
                { "under_5", "Under 5 years" },
                { "5_to_7", "5 to 7 years" },
                { "above_7", "Above 7 years" },

                // Skill categories
                { "english_learning", "English Learning" },
                { "attention_strengthening", "Attention Strengthening" },
                { "math_learning", "Math Learning" },
                { "memory_strengthening", "Memory Strengthening" },
                { "farsi_learning", "Farsi Learning" },
                { "skill_learning", "Skill Learning" },
                { "school_readiness", "School Readiness" },
                { "mobile_usage", "Mobile Usage" },

                // Game learning description
                { "game_learning_title", "Learn by playing games" },
                { "game_learning_description", "Between 3-7 skill games daily that are personalized to help your child improve skills" },

                // Parent verification screen
                { "parent_verification_title", "This is for parents and guardians" },
                { "enter_numbers_instruction", "Please enter the following numbers in order" },

                // Subscription details
                { "mabube_review", "Noroland is the only mobile game that is truly useful and educational." },
                { "privacy_policy", "Privacy Policy" },
                { "continue_with_plan", "Continue with this plan" },
                { "monthly_label_3m", "Monthly" },
                { "affordable", "Affordable" },

                // Onboarding progress
                { "personalized_experience_detail", "Based on your child's needs, we've prepared a customized learning path" },
                { "plan_selection_detail", "Select an appropriate plan" },
                { "adventure_start_detail", "Access unlimited games in language and math to strengthen life skills" },
                { "progress_tracking_detail", "Track your child's progress step by step with Noroland" },
                { "trial_offer", "Try Noroland for 3 months" },

                // Contact info screen
                { "stay_updated", "Stay updated on (NAME)'s progress" },
                { "info_privacy_note", "Your information is confidential. We'll only send your child's performance reports." },
                { "email_phone_placeholder", "Email or phone number" },
                { "do_later", "I'll do this later" }
            };
        }

        public string GetLocalizedValue(string key)
        {
            return localizedTexts.TryGetValue(key, out string value) ? value : key;
        }
    }

  
}
