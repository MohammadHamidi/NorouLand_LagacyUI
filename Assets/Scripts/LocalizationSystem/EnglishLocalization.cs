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
    // Basic numbers
    { "number_1", "1" },
    { "number_2", "2" },
    { "number_3", "3" },
    { "number_4", "4" },
    { "number_5", "5" },
    { "number_6", "6" },
    { "number_7", "7" },
    { "number_8", "8" },
    { "number_9", "9" },
    { "number_0", "0" },

    // Welcome Screen
    { "welcome_title", "Welcome to Noroland" },
    { "app_description", "Noroland" },
    { "get_ready", "Getting ready to make learning fun" },
    { "who_uses_app", "Who is here??" },
    { "child_option", "Child" },
    { "parent_option", "Parent" },
    { "save_60_percent", "Save <color=#FFA500>60%</color> on the yearly plan" },

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
    { "continue_progress", "Continue" },
    { "select_all_that_apply", "Select everything that applies." },

    // Graph 
    { "memory", "Memory" },
    { "language", "Language" },
    { "life_skills", "Life Skills" },
    { "math_logic", "Math and logic" },
    { "attention", "Attention" },

    // Buttons
    { "view_all_plans", "View All Plans" },
    { "start_3_day_trial", "Start 3-Day Free Trial" },
    { "next_button", "Next" },
    { "age_personalization_subtitle", "We'll personalize the experience for this age." },

    // Parent info screen
    { "parent_info_title", "Information for Parents and Guardians" },
    { "info_summary", "To all informed parents and guardians, welcome to Noroland!" },
    { "count_prompt", "Count: One, Two, Three" },
    { "progress_percent", "0%" },
    { "personalization_message", "Personalizing the learning path" },

    // Child info questions (Basic)
    { "child_name_question", "What is your child's name?" },
    { "child_age_question", "How old is (NAME)?" },
    { "help_areas_question", "How can Noroland help?" },

    // New keys for Child Name Screen
    { "child_name_subtitle", "Because every explorer has a name." },
    { "child_name_input_placeholder", "Your child’s name" },
    { "child_name_button", "Continue" },

    // New keys for Child Age Screen
    { "child_age_subtitle", "We’ll personalize the experience for this age." },
    { "child_age_button", "Continue" },

    // New keys for Help Areas Screen
    { "help_areas_subtitle", "Select everything that applies." },
    { "help_areas_button", "Continue" },

    // Age options
    { "under_5", "Under 5 years" },
    { "5_to_7", "5 to 7 years" },
    { "above_7", "Above 7 years" },

    // Skill categories (updated per JSON)
    { "english_learning", "Learn English" },
    { "attention_strengthening", "Develop Attention Skills" },
    { "math_and_logic", "Learn Math and Logic" },
    { "memory_strengthening", "Improve Memory" },
    { "farsi_learning", "Learn Persian" },
    { "skill_learning", "Develop Life Skills" },
    { "school_readiness", "School Readiness" },
    { "mobile_usage", "Productive screen time" },

    // Game learning description
    { "game_learning_title", "Learn by playing games" },
    { "game_learning_description", "Between 3-7 skill games daily that are personalized to help your child improve skills" },
    {"tailored","Tailored Just for Them"},
    {"tailored_s","Personalized games for their age, skills, and interests. Because every child is unique."},
    // Parent verification screen (Basic)
    { "parent_verification_title", "This is for parents and guardians" },
    { "enter_numbers_instruction", "Please enter the following numbers in order" },

    // New keys for Secure Your Progress Screen
    { "secure_progress_title", "Secure your progress" },
    { "secure_progress_description", "Your data is safe with us. only used for progress updates." },
    { "secure_progress_input_placeholder", "Phone number" },
    { "secure_progress_button_remind_later", "Remind me later" },
    { "secure_progress_button_continue", "Continue" },

    // New keys for Learn while Playing Screen
    { "learn_play_language_selector", "English" },
    { "learn_play_title", "Learn while playing" },
    { "learn_play_description", "+40 cognitive games designed to entertain your kid while boosting essential skills." },
    { "learn_play_button", "Continue" },

    // New keys for Learning Scores / Chart Screen
    { "learning_scores_title", "Neuroland Improves Learning Scores by 40%" },
    { "learning_scores_chart_left", "Kids not playing our games" },
    { "learning_scores_chart_right", "Kids playing Neuroland" },
    { "learning_scores_learn_more", "Learn more about our researches" },
    { "learning_scores_button", "Continue" },

    // New keys for Invest in [Name]’s Future Screen
    { "invest_future_title", "Invest in [Name]’s Future Today" },
    { "invest_future_step1_title", "Personalize your journey" },
    { "invest_future_step1_description", "We created a tailored experience based on your child's needs." },
    { "invest_future_step2_title", "Pick Your Plan" },
    { "invest_future_step2_description", "Choose the plan that works for you." },
    { "invest_future_step3_title", "Let the Adventure Begin" },
    { "invest_future_step3_description", "Access unlimited games that boost skills in math, language, life skills and more." },
    { "invest_future_step4_title", "See the Progress" },
    { "invest_future_step4_description", "Watch your child grow step by step with Neuroland by their side." },
    { "invest_future_button_view_plans", "View All Plans" },
    { "invest_future_button_subscribe", "Get 3 Months of Adventure" },

    // New keys for Save 60% on the Yearly Plan Screen
    { "plan_screen_title", "Save 60% on the yearly plan" },
    { "plan_screen_plan1_duration", "1 Month" },
    { "plan_screen_plan1_price", "59.000T" },
    { "plan_screen_plan2_duration", "3 Months" },
    { "plan_screen_plan2_label", "Most Popular" },
    { "plan_screen_plan2_original_price", "129.000T" },
    { "plan_screen_plan2_discounted_price", "115.000T" },
    { "plan_screen_plan2_monthly_price", "38.000T/Month" },
    { "plan_screen_plan3_duration", "1 Year" },
    { "plan_screen_plan3_label", "Economical" },
    { "plan_screen_plan3_original_price", "399.000T" },
    { "plan_screen_plan3_d", "340.000T" },
    { "plan_screen_plan3_monthly_price", "28.000T/Month" },
    { "plan_screen_review_name", "Mahboube" },
    { "plan_screen_review_rating", "★★★★★" },
    { "plan_screen_review_comment", "The only mobile game that's actually useful and educational." },
    { "plan_screen_privacy_policy", "Privacy Policy" },
    { "plan_screen_button", "Continue" },

    // New keys for Congrats Screen
    { "congrats_title", "CONGRATS" },
    { "congrats_subtitle", "YOU'RE A WISE PARENT!" },
    { "congrats_instruction", "Now hand the phone to [Name]." },
    { "congrats_button", "Start Playing Now" },

    // New keys for Personalizing [Name]’s Experience Screen
    { "personalizing_title", "Personalizing [Name]’s experience" },
    { "personalizing_progress", "0%" },
    { "personalizing_step1", "Analyzing goals" },
    { "personalizing_step2", "Adjusting difficulty" },
    { "personalizing_step3", "Customizing games" },

    // New keys for Grown-ups Only / Parent Verification Screen
    { "grownups_title", "Grown-ups only!" },
    { "grownups_instruction", "Please enter the following numbers" },
    { "grownups_numbers", "Three, Four, Eight, Nine" },
    { "grownups_input_placeholder", "_ _ _ _" },
    { "grownups_keypad", "1, 2, 3, 4, 5, 6, 7, 8, 9, 0, ←" }
};

        }

        public string GetLocalizedValue(string key)
        {
            return localizedTexts.TryGetValue(key, out string value) ? value : key;
        }
    }

  
}
