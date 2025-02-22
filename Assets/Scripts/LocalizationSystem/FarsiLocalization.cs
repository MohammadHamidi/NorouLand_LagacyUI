using System.Collections.Generic;

namespace LocalizationSystem
{
      public class FarsiLocalization : ILocalization
    {
        public string Language => "Farsi";
        private Dictionary<string, string> localizedTexts;

        public FarsiLocalization()
        {
            LoadLocalizationData();
        }

        public void LoadLocalizationData()
        {
            localizedTexts = new Dictionary<string, string>
            {
                // Welcome Screen
                { "welcome_title", "به نورولند خوش آمدی" },
                { "app_description", "نورولند" },
                { "get_ready", "بازی کن، باهوش شو" },
                { "who_uses_app", "چه کسی از برنامه استفاده میکند؟" },
                { "child_option", "کودک" },
                { "parent_option", "والدین" },

                // Subscription Screen
                { "today_investment", "امروز روی آینده‌ی (اسم) سرمایه‌گذاری کن" },
                { "save_percent", "با پلن سالانه ٪۴۵ کمتر هزینه کن" },

                // Plan Options
                { "one_month", "یک ماهه" },
                { "three_months", "۳ ماهه" },
                { "one_year", "یک ساله" },
                { "monthly_price_1m", "۵۹ هزارتومان" },
                { "monthly_price_3m", "۴۰ هزارتومان" },
                { "monthly_price_1y", "۳۲٬۵ هزارتومان" },
                { "total_price_1m", "۵۹ هزارتومان" },
                { "total_price_3m", "۱۲۰ هزارتومان" },
                { "total_price_1y", "۳۹۰ هزارتومان" },
                { "most_popular", "محبوب‌ترین" },
                { "best_offer", "بهترین پیشنهاد" },

                // Onboarding Steps
                { "child_personalization", "شخصی‌سازی تجربه کودک" },
                { "select_age", "انتخاب سن مناسب" },
                { "start_learning", "شروع یادگیری" },
                { "continue_progress", "مشاهده پیشرفت" },

                // Buttons
                { "view_all_plans", "مشاهده همه پلن‌ها" },
                { "start_3_day_trial", "۳ روز رایگان نورولند را امتحان کنید" },
                { "next_button", "بعدی" },

                // Parent info screen
                { "parent_info_title", "ابتدا برای مامان و باباهاست" },
                { "info_summary", "به جمع والدین آگاه و باهوش نورولند خوش آمدید" },
                { "count_prompt", "سه، پنج، شش، نه" },
                { "progress_percent", "٪۰" },
                { "personalization_message", "در حال شخصی سازی مسیر یادگیری (اسم)" },
                // Graph 
                { "memory", "حافظه" },
                { "language", "زبان" },
                { "life_skills", "مهارت های نرم" },
                { "math_and_logic", "ریاضی" },
                { "attention", "توجه" },
                
                // Child info questions
                { "child_name_question", "اسم فرزندتون چیه؟" },
                { "child_age_question", "(اسم) چند ساله است؟" },
                { "help_areas_question", "نورولند چطور میتونه بهتون کمک کنه؟" },

                // Age options
                { "under_5", "زیر ۵ سال" },
                { "5_to_7", "۵ تا ۷ سال" },
                { "above_7", "بالای ۷ سال" },

                // Skill categories
                { "english_learning", "آموزش انگلیسی" },
                { "attention_strengthening", "تقویت توجه" },
                { "math_learning", "آموزش ریاضی" },
                { "memory_strengthening", "تقویت حافظه" },
                { "farsi_learning", "آموزش فارسی" },
                { "skill_learning", "مهارت‌آموزی" },
                { "school_readiness", "آماده‌سازی برای مدرسه" },
                { "mobile_usage", "استفاده مفید از موبایل" },

                // Game learning description
                { "game_learning_title", "با بازی کردن یاد بگیر" },
                { "game_learning_description", "بین ۳ تا ۷ بازی مهارتی که بر اساس نیاز فرزندتان شخصی‌سازی می‌شود تا مهارت‌هایش را تقویت کند" },

                // Parent verification screen
                { "parent_verification_title", "اینجا برای مامانا و باباهاست" },
                { "enter_numbers_instruction", "لطفا اعداد زیر رو به ترتیب وارد کنید." },

                // Subscription details
                { "mabube_review", "نورولند تنها بازی موبایلیه که واقعا مفید و آموزشیه." },
                { "privacy_policy", "حفظ حریم خصوصی" },
                { "continue_with_plan", "ادامه با این پلن" },
                { "monthly_label_3m", "ماهیانه" },
                { "affordable", "مقرون به صرفه" },

                // Onboarding progress
                { "personalized_experience_detail", "بر اساس نیازهای فرزندت، مسیر مخصوص خودش رو آماده کردیم" },
                { "plan_selection_detail", "پلن مناسب رو انتخاب کن" },
                { "adventure_start_detail", "به بازی‌های نامحدود دسترسی پیدا کن و از زبان و ریاضی تا مهارت‌های زندگی رو تقویت کن." },
                { "progress_tracking_detail", "رشد فرزندت رو قدم به قدم دنبال کن، نورولند توی تمام این مسیر همراهته." },
                { "trial_offer", "۳ ماه با نورولند همراه باش" },

                // Contact info screen
                { "stay_updated", "در جریان پیشرفت (اسم)؟ باش" },
                { "info_privacy_note", "اطلاعات شما پیش ما میمونه. ما فقط گزارش عملکرد فرزندتون رو پیامک میکنیم." },
                { "email_phone_placeholder", "ایمیل یا شماره تلفن" },
                { "do_later", "بعدا انجام میدم" }
            };
        }

        public string GetLocalizedValue(string key)
        {
            return localizedTexts.TryGetValue(key, out string value) ? value : key;
        }
    }
}