namespace Microsoft.AspNetCore.Appleseed
{
    public class Input_vertical_separation
    {
        private const string transparent = "border:none; background-color: transparent; ";
        public const string very_small = transparent + "height: 2px;";
        public const string small = transparent + "height: 5px;";
        public const string medium = transparent + "height: 10px;";
        public const string big = transparent + "height: 15px;";
        public static string Get_specific_height(int height_in_px)
        {
            return transparent + "height: " + height_in_px + "px;";
        }
    }
    public class Input_horizontal_separation
    {
        private const string transparent = "border:none; background-color: transparent; ";
        public const string very_very_small = transparent + "width: 3px;";
        public const string very_small = transparent + "width: 6px;";
        public const string small = transparent + "width: 12px;";
        public const string medium = transparent + "width: 22px;";
        public const string big = transparent + "width: 32px;";
        public const string very_big = transparent + "width: 42px;";
        public static string Get_specific_width(int width_in_px)
        {
            return transparent + "width: " + width_in_px + "px;";
        }
    }
}
