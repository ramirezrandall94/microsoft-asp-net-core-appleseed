using Microsoft.JSInterop;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Appleseed
{
    public class JS_JSON_Class_Base_Base
    {
    }
    public class JS_JSON_Class_Base<inheritor_type> : JS_JSON_Class_Base_Base
        where inheritor_type : class
    {
        public string ConvertToJSONObject(inheritor_type inheritor_instance)
        {
            string json_object = "{";
            Type inheritor_type = typeof(inheritor_type);
            FieldInfo[] field_infos = inheritor_type.GetFields(BindingFlags.Public
                | BindingFlags.Instance | BindingFlags.Static);
            for (int field_infos_index = 0; field_infos_index < field_infos.Length;
                ++field_infos_index)
            {
                FieldInfo current_field_info = field_infos[field_infos_index];
                if (current_field_info.FieldType.BaseType
                    == typeof(JS_Variable_Base))
                {
                    object js_variable = current_field_info.GetValue(inheritor_instance);
                    JS_Variable_Base js_variable_base = (JS_Variable_Base)js_variable;
                    json_object += js_variable_base.GetAsJSONObjectLeftAndRightValue();
                }
                else
                {
                    throw new Exception("Error: JS JSON Classes can only contain"
                        + " JS_Variable variables as members...");
                }
            }
            json_object += "}";
            return json_object;
        }
    }
    public class JS_Variable_Base
    {
        public JS_Variable_Base(string nameof_variable, object? initial_value,
            Type variable_type)
        {
            if (initial_value?.GetType().BaseType == typeof(JS_Variable_Base))
            {
                throw new Exception("Error: variable_type cannot be another JS_Variable...");
            }
            if(variable_type == typeof(string)
                || variable_type == typeof(int) || variable_type == typeof(int?)
                || variable_type == typeof(double) || variable_type == typeof(double?)
                || variable_type.BaseType.BaseType == typeof(JS_JSON_Class_Base_Base))
            {

            }
            else
            {
                throw new Exception("Error: variable_type can only be string, int, double or" +
                    " JS_JSON_Class_Base...");
            }
            _Name = nameof_variable;
            _Value = initial_value;
        }
        protected string _Name = "";
        protected object? _Value = null;
        public string GetAsJSONObjectLeftAndRightValue()
        {
            string stringified = "";
            if (_Value == null)
            {
                stringified = _Name + ":null,";
            }
            else if (_Value != null)
            {
                if (_Value.GetType() == typeof(string))
                {
                    stringified = _Name + ":`" + _Value + "`,";
                }
                else if (_Value.GetType() == typeof(int) || _Value.GetType() == typeof(double))
                {
                    stringified = _Name + ":" + _Value + ",";
                }
                else if (_Value.GetType().BaseType.BaseType == typeof(JS_JSON_Class_Base_Base))
                {
                    MethodInfo ConvertToJSONObjectMethod =
                        _Value.GetType().GetMethod("ConvertToJSONObject");
                    string value_as_json =
                        (string)ConvertToJSONObjectMethod.Invoke(_Value, new object[] { _Value });
                    stringified = _Name + ":" + value_as_json + ",";
                }
            }
            return stringified;
        }
        public string GetAsJSONObjectAsRightValue()
        {
            string stringified = "";
            if (_Value == null)
            {
                stringified = _Name + ":null,";
            }
            else if (_Value != null)
            {
                if (_Value.GetType() == typeof(string))
                {
                    stringified = "`" + _Value + "`";
                }
                else if (_Value.GetType() == typeof(int) || _Value.GetType() == typeof(double))
                {
                    stringified = _Value.ToString();
                }
                else if (_Value.GetType().BaseType.BaseType == typeof(JS_JSON_Class_Base_Base))
                {
                    MethodInfo ConvertToJSONObjectMethod =
                        _Value.GetType().GetMethod("ConvertToJSONObject");
                    string value_as_json =
                        (string)ConvertToJSONObjectMethod.Invoke(_Value, new object[] { _Value });
                    stringified = value_as_json;
                }
            }
            return stringified;
        }
    }
    public class JS_Variable<variable_type> : JS_Variable_Base
    {
        public JS_Variable(string nameof_variable, variable_type? initial_value)
            : base(nameof_variable, initial_value, typeof(variable_type))
        {

        }
        public string Name
        {
            get
            {
                return _Name;
            }
        }
        public variable_type? Value
        {
            get
            {
                return (variable_type?)_Value;
            }
            set
            {
                _Value = value;
            }
        }
    }
    public class JS_Function_Name
    {
        /// <summary>
        /// The Guid is going to be appended to Function_Name
        /// </summary>
        /// <param name="Function_Name"></param>
        /// <param name="append_to_function_name"></param>
        public JS_Function_Name(string Function_Name)
        {
            _Function_Name = Function_Name;
        }
        private string _Function_Name = "";
        public string Function_Name
        {
            get
            {
                return _Function_Name;
            }
        }
    }
    public class JS_Parameter_Value<parameter_type, parameter_name>
    {
        public JS_Parameter_Value(parameter_type parameter_1_value)
        {
            this.parameter_1_value = parameter_1_value;
        }
        public parameter_type parameter_1_value;
    }
    public class JS_Parameter<parameter_type, parameter_name>
    {
        public JS_Parameter()
        {
            _Name = typeof(parameter_name).Name;
        }
        private string _Name = "";
        public string Name
        {
            get
            {
                return _Name;
            }
        }
    }
    public class JS_Function_Body<parameter_type, parameter_name>
    {

    }
    public class JS_Function<parameter_type, parameter_name, return_type>
        where parameter_type : JS_JSON_Class_Base_Base, new()
        where return_type : JS_JSON_Class_Base_Base, new()
    {
        public JS_Function(JS_Function_Name name)
        {
            _Function_Name = name;
            _Return_Type = typeof(return_type);
        }
        private JS_Parameter<parameter_type, parameter_name>
            _Parameter =
            new JS_Parameter<parameter_type, parameter_name>();
        public JS_Parameter
            <parameter_type, parameter_name> Parameter

        {
            get
            {
                return _Parameter;
            }
        }
        public async Task<return_type> Call_From_C_Sharp(parameter_type parameter_value, IJSRuntime JSRuntime)
        {

            string value_as_json =
                await JSRuntime.InvokeAsync<string>(_Function_Name.Function_Name,
            parameter_value);
            return_type value_to_return = ParseJSON(value_as_json);
            return value_to_return;
        }
        private return_type ParseJSON(string value_as_json)
        {
            return_type value_to_return = new return_type();
            FieldInfo[] fields = typeof(return_type).GetFields(BindingFlags.Public
    | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                string field_name = field.Name;
                object field_object = field.GetValue(value_to_return);
                PropertyInfo value_property = field.FieldType.GetProperty("Value");
                if (field.FieldType == typeof(JS_Variable<string?>)
                    || field.FieldType == typeof(JS_Variable<string>))
                {
                    ParseString(field_name, value_as_json, value_property, field_object);
                }
                else if (field.FieldType == typeof(JS_Variable<int?>)
                    || field.FieldType == typeof(JS_Variable<int>))
                {
                    ParseInt(field_name, value_as_json, value_property, field_object);
                }
                else if (field.FieldType == typeof(JS_Variable<double>)
                    || field.FieldType == typeof(JS_Variable<double?>))
                {
                    ParseDouble(field_name, value_as_json, value_property, field_object);
                }
            }
            return value_to_return;
        }
        private void ParseString(string field_name, string value_as_json,
            PropertyInfo value_property, object field_object)
        {
            Regex string_field_regex = new Regex($"\"{field_name}\":\"(?<string_value>[nullA-Za-z0-9!@#$%^&*()_+\\-=|;':,./<>?`~]*)\"");
            Match match = string_field_regex.Match(value_as_json);
            string match_as_string = match.Groups["string_value"].ToString();
            if (match_as_string == "null")
            {
                value_property.SetMethod.Invoke(field_object, new object?[] { null });
            }
            else if (match_as_string != "null")
            {
                value_property.SetMethod.Invoke(field_object, new object[] { match_as_string });
            }
        }
        private void ParseInt(string field_name, string value_as_json,
            PropertyInfo value_property, object field_object)
        {
            Regex number_field_regex = new Regex($"\"{field_name}\":(?<int_value>[0-9null]+)");
            Match match = number_field_regex.Match(value_as_json);
            string match_as_string = match.Groups["int_value"].ToString();
            if (match_as_string == "null")
            {
                value_property.SetMethod.Invoke(field_object, new object?[] { null });
            }
            else if (match_as_string != "null")
            {
                int match_as_int = int.Parse(match_as_string);
                value_property.SetMethod.Invoke(field_object, new object[] { match_as_int });
            }
        }
        private void ParseDouble(string field_name, string value_as_json,
    PropertyInfo value_property, object field_object)
        {
            Regex number_field_regex = new Regex($"\"{field_name}\":(?<double_value>[0-9.null]+)");
            Match match = number_field_regex.Match(value_as_json);
            string match_as_string = match.Groups["double_value"].ToString();
            if (match_as_string == "null")
            {
                value_property.SetMethod.Invoke(field_object, new object?[] { null });
            }
            else if (match_as_string != "null")
            {
                double match_as_double = double.Parse(match_as_string);
                value_property.SetMethod.Invoke(field_object, new object[] { match_as_double });
            }
        }
        public async Task<bool> Call_From_C_Sharp_Void(parameter_type parameter_value, IJSRuntime JSRuntime)
        {
            await JSRuntime.InvokeVoidAsync(_Function_Name.Function_Name, parameter_value);
            return true;
        }
        public async Task<bool> Call_From_C_Sharp_Void(IJSRuntime JSRuntime)
        {
            await JSRuntime.InvokeVoidAsync(_Function_Name.Function_Name);
            return true;
        }
        public string Get_call_as_string(parameter_type parameter_value)
        {
            MethodInfo StringifyWholeClassMethod = parameter_value.GetType().GetMethod("ConvertToJSONObject");
            string function_call = Function_Name.Function_Name + "(";
            function_call += StringifyWholeClassMethod.Invoke(parameter_value, new object[] { parameter_value });
            function_call += ")";
            return function_call;
        }
        public string Get_call_as_string()
        {
            string function_call = Function_Name.Function_Name + "()";
            return function_call;
        }
        private JS_Function_Name? _Function_Name = null;
        public JS_Function_Name? Function_Name
        {
            get
            {
                return _Function_Name;
            }
        }
        private Type? _Return_Type = null;
        private bool _Is_return_type_void = false;
        public bool Is_return_type_void
        {
            get
            {
                return _Is_return_type_void;
            }
        }
        public Type? Return_Type
        {
            get
            {
                return _Return_Type;
            }
        }
    }
}
