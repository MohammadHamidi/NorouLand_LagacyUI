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
    // Basic numbers
    { "number_1", "۱" },
    { "number_2", "۲" },
    { "number_3", "۳" },
    { "number_4", "۴" },
    { "number_5", "۵" },
    { "number_6", "۶" },
    { "number_7", "۷" },
    { "number_8", "۸" },
    { "number_9", "۹" },
    { "number_0", "۰" },

    // Welcome Screen
    { "welcome_title", "به نورولند خوش آمدی" },
    { "app_description", "نورولند" },
    { "get_ready", "بازی کن، باهوش شو" },
    { "who_uses_app", "چه کسی از برنامه استفاده میکند؟" },
    { "child_option", "کودک" },
    { "parent_option", "والدین" },
    { "save_60_percent", "با پلن سالانه <color=#FFA500>۶۰٪</color> کمتر هزینه کن" },

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
    {"tailored","شخصی سازی برای هر کودک"},
    {"tailored_s","مسیر و بازی های شخصی سازی شده بر اساس سن، هدف، و علایق هر کودک."},
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
    { "math_logic", "ریاضی" },
    { "attention", "توجه" },

    // Child info questions (Basic)
    { "child_name_question", "اسم فرزندتون چیه؟" },
    { "child_age_question", "(اسم) چند ساله است؟" },
    { "help_areas_question", "نورولند چطور میتونه بهتون کمک کنه؟" },

    // New keys for Child Name Screen
    { "child_name_subtitle", "هر کاوشگری یه اسم داره." },
    { "child_name_input_placeholder", "اسم فرزندتون" },
    { "child_name_button", "بعدی" },

    // New keys for Child Age Screen
    { "child_age_subtitle", "ما مسیر رو بر اساس سن کودک شخصی سازی می‌کنیم." },
    { "child_age_button", "بعدی" },

    // New keys for Help Areas Screen
    { "help_areas_subtitle", "تمام مواردی که میخواین رو انتخاب کنید." },
    { "help_areas_button", "بعدی" },

    // Age options
    { "under_5", "زیر ۵ سال" },
    { "5_to_7", "۵ تا ۷ سال" },
    { "above_7", "بالای ۷ سال" },

    // Skill categories (updated per JSON)
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

    // Parent verification screen (Basic)
    { "parent_verification_title", "اینجا برای مامانا و باباهاست" },
    { "enter_numbers_instruction", "لطفا اعداد زیر رو به ترتیب وارد کنید." },

    // New keys for Secure Your Progress Screen
    { "secure_progress_title", "در جریان پیشرفت {اسم} باش" },
    { "secure_progress_description", "اطلاعات شما پیش ما امنه. ما فقط گزارش عملکرد فرزندتون رو پیامک میکنیم." },
    { "secure_progress_input_placeholder", "ایمیل یا شماره تلفن" },
    { "secure_progress_button_remind_later", "بعدا انجام میدم" },
    { "secure_progress_button_continue", "بعدی" },

    // New keys for Learn while Playing Screen
    { "learn_play_language_selector", "Farsi" },
    { "learn_play_title", "با بازی کردن یاد بگیر" },
    { "learn_play_description", "بیش از ۴۰ بازی شناختی که هم سرگرم‌کننده‌ست و هم مهارت‌های ضروری رو تقویت می‌کنه." },
    { "learn_play_button", "بعدی" },

    // New keys for Learning Scores / Chart Screen
    { "learning_scores_title", "نورولند مهارت‌های یادگیری رو تا ٪۴۰ افزایش میده" },
    { "learning_scores_chart_left", "بچه‌هایی که نورولند بازی نکردن" },
    { "learning_scores_chart_right", "بچه‌هایی که نورولند بازی کردن" },
    { "learning_scores_learn_more", "اطلاعات بیشتر درباره تحقیقات ما" },
    { "learning_scores_button", "بعدی" },

    // New keys for Invest in [Name]’s Future Screen
    { "invest_future_title", "امروز روی آینده‌ی {اسم} سرمایه‌گذاری کن" },
    { "invest_future_step1_title", "شخصی سازی تجربه کودک" },
    { "invest_future_step1_description", "بر اساس نیازهای فرزندت، مسیر مخصوص خودش رو آماده کردیم." },
    { "invest_future_step2_title", "انتخاب پلن مناسب" },
    { "invest_future_step2_description", "پلن مناسب رو انتخاب کن." },
    { "invest_future_step3_title", "شروع ماجراجویی" },
    { "invest_future_step3_description", "با بازی‌های نامحدود، مهارت پیدا کن، ریاضی و زبانت رو تقویت کن." },
    { "invest_future_step4_title", "مشاهده پیشرفت" },
    { "invest_future_step4_description", "رشد فرزندت رو قدم به قدم دنبال کن. نورولند توی تمام این مسیر همراهته." },
    { "invest_future_button_view_plans", "مشاهده همه پلن‌ها" },
    { "invest_future_button_subscribe", "۳ ماه با نورولند همراه باش" },

    // New keys for Save 60% on the Yearly Plan Screen
    { "plan_screen_title", "با پلن سالانه ۶۰٪ کمتر هزینه کن" },
    { "plan_screen_plan1_duration", "یک ماهه" },
    { "plan_screen_plan1_price", "۵۹ هزار تومان" },
    { "plan_screen_plan2_duration", "۳ ماهه" },
    { "plan_screen_plan2_label", "محبوب‌ترین" },
    { "plan_screen_plan2_original_price", "۱۲۹ هزار تومان" },
    { "plan_screen_plan2_discounted_price", "۱۱۵ هزار تومان" },
    { "plan_screen_plan2_monthly_price", "ماهانه ۳۸ هزار تومان" },
    { "plan_screen_plan3_duration", "یک ساله" },
    { "plan_screen_plan3_label", "مقرون به صرفه" },
    { "plan_screen_plan3_original_price", "۳۹۹ هزار تومان" },
    { "plan_screen_plan3_discounted_price", "۳۴۰ هزار تومان" },
    { "plan_screen_review_name", "محبوبه" },
    { "plan_screen_review_rating", "★★★★★" },
    { "plan_screen_review_comment", "نورولند تنها بازی موبایله که واقعا مفید و آموزنده‌ست." },
    { "plan_screen_privacy_policy", "حفظ حریم خصوصی" },
    { "plan_screen_button", "ادامه با این پلن" },

    // New keys for Congrats Screen
    { "congrats_title", "به جمع والدین آگاه با فرزندان باهوش نورولند خوش اومدید" },
    { "congrats_instruction", "سفر یادگیری {اسم} از همین حالا شروع میشه." },
    { "congrats_button", "بریم بازی کنیم" },

    // New keys for Personalizing [Name]’s Experience Screen
    { "personalizing_title", "در حال شخصی سازی مسیر یادگیری {اسم}" },
    { "personalizing_progress", "۰٪" },
    { "personalizing_step1", "بررسی اهداف" },
    { "personalizing_step2", "تنظیم سطح سختی" },
    { "personalizing_step3", "شخصی سازی بازی‌ها" },

    // New keys for Grown-ups Only / Parent Verification Screen
    { "grownups_title", "اینجا برای مامانا و باباهاست" },
    { "grownups_instruction", "لطفا اعداد زیر رو به ترتیب وارد کنید." },
    { "grownups_numbers", "سه, پنج, شش, نه" },
    { "grownups_input_placeholder", "_ _ _ _" },
    { "grownups_keypad", "۱, ۲, ۳, ۴, ۵, ۶, ۷, ۸, ۹, ۰, ←" }
};

        }

        public string GetLocalizedValue(string key)
        {
            return localizedTexts.TryGetValue(key, out string value) ? value : key;
        }
    }
}