using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Appleseed
{
    public class Font_Sizes
    {
        // Fonts sizes displayed link: https://localhost/visual_studio/avanza_blazor/standard_font_sizes.html
        public const string very_very_small = "10px";
        public const string very_small = "14px";
        public const string @base_small = "18px";
        public const string medium = "25px";
        public const string big = "32px";
        public const string pretitle = "24px";
        public const string title = "38px";
        public const string subtitle = "24px";
        public static string get_only_pixels(string text_size)
        {
            Regex regex = new Regex(@"\d+px");
            return regex.Match(text_size).Value;
        }
    }

    public class CSS_Class
    {
        /// <summary>
        /// The Guid is going to be appended to Class_Name
        /// </summary>
        /// <param name="Class_Name"></param>
        /// <param name="append_to_class_name"></param>
        public CSS_Class(string Class_Name, Guid append_to_class_name)
        {
            _Class_name = Class_Name + class_name_uid_part_separator + append_to_class_name.ToString();
        }
        private string _Class_name = string.Empty;
        private const string class_name_uid_part_separator = "__uid__";
        public string Class_Name
        {
            get
            {
                return _Class_name;
            }
        }
        private List<CSS_Class> _CSS_Classes = new List<CSS_Class>();
        public void Add_CSS_Class(CSS_Class css_class)
        {
            _CSS_Classes.Add(css_class);
        }
        public void Remove_CSS_Class(int at_index)
        {
            _CSS_Classes.RemoveAt(at_index);
        }
        public CSS_Class Get_CSS_Class(int at_index)
        {
            return _CSS_Classes[at_index];
        }
        public int CSS_Classes_Count
        {
            get
            {
                return _CSS_Classes.Count;
            }
        }
        public string All_CSS_Classes_Names
        {
            get
            {
                string css_classes_string = Class_Name + " ";
                foreach(CSS_Class css_class in _CSS_Classes)
                {
                    css_classes_string += css_class.All_CSS_Classes_Names + " ";
                }
                return css_classes_string;
            }
        }
    }
}
