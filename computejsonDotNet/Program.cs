using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp.Scripting;
namespace Filter
{
    public class Program
    {
        static string filterFlag = "--compute=";
        //Creating stream and checkiing FSON for completeness
        static void Main(string[] args)
        {
            int recordNumber = 0;
            var filters = gettingPar(args);
            using (FileStream s = File.Open(args[0], FileMode.Open))
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                int beginIndex = 0;
                int endIndex = 0;
                int beginJson = 0;
                string thisJson = "";
                while (!sr.EndOfStream)
                {
                    var jsonInput = sr.ReadLine();
                    while (!String.IsNullOrEmpty(jsonInput))
                    {
                        if (beginJson == 0)
                        {
                            if (jsonInput.Contains("{"))
                            {
                                beginIndex = jsonInput.IndexOf("{");
                                beginJson = 1;
                                jsonInput = jsonInput.Substring(beginIndex, jsonInput.Length - beginIndex);
                            }
                            else
                            {
                                jsonInput = "";
                            }
                        }
                        else
                        {
                            if (jsonInput.Contains("}"))
                            {
                                endIndex = jsonInput.IndexOf("}");
                                thisJson = thisJson + jsonInput.Substring(0, endIndex + 1);
                                recordNumber++;
                                jsonInput = jsonInput.Substring(endIndex + 1, jsonInput.Length - endIndex - 1);
                                ifArgIsStringMethod(thisJson, filters, recordNumber);
                                IfArgsAreMath(thisJson, filters, recordNumber);
                                thisJson = "";
                                beginJson = 0;
                            }
                            else
                            {
                                thisJson = thisJson + jsonInput;
                                jsonInput = "";
                            }
                        }
                    }
                }
            }
        }
        //Removing a flag and convert string result to array 
        public static string[] gettingPar(string[] args)
        {
            string result = "";
            for (var c = 1; c <= args.Length; c++)
            {
                if (args[c].StartsWith(filterFlag))
                {
                    result = args[c].Replace(filterFlag, "");
                    break;
                }
                else
                {
                    Console.WriteLine($"Command flag {args[c]} is incorrect. Should be: --filter=");
                }
            }
            String[] resultArray = result.Split(' ');
            return resultArray;
        }
        //Calling this method to use string methods  
        public async static void ifArgIsStringMethod(string thisJson, string[] filterText, int recordNumber)
        {
            if (filterText.Count() < 2)
            {
                string i = string.Join(' ', filterText);
                //Receiving substrings for JSON path and Lambada expression
                string substrForLamb = i.Substring(i.IndexOf("."), i.IndexOf(")") - i.IndexOf(".") + 1);
                string substrForPath = i.Substring(i.IndexOf("@"), i.IndexOf("."));  
                //Replasing indicators for JSONPath
                string replaceIndicators = substrForPath.Replace("@@", "");
                //Generating JSON path by template and use it to get value
                string pathJS = $"$.{replaceIndicators}";
                JObject source = JObject.Parse(thisJson);
                JToken value = source.SelectToken(pathJS);
                string valueJson = value.ToString();
                //Generating Lambada expression by template and use Roslyn approach to get output
                string addTo = $"f => f{substrForLamb}";
                string lambEx = addTo;
                var rosolyn = await CSharpScript.EvaluateAsync<Func<string, object>>(lambEx);
                string stringResult = rosolyn(valueJson).ToString();

                if (stringResult == "-1" || stringResult == valueJson)//For Substring() and IndexOf() methods
                {
                    Console.WriteLine($"String: {recordNumber} Method: {substrForLamb} returned False");
                }
                else
                {
                    Console.WriteLine($"String: {recordNumber} Result: {stringResult}");
                }
            }
        }
        //Calling this method for computation   
        public static string IfArgsAreMath(string thisJson, string[] filterText, int recordNumber)
        {
            string stringResult = "";
            if (filterText.Count() > 1)
            {
                foreach (string i in filterText)
                {
                    if (i.Contains("@@"))
                    {   //Replacing indicators for JSONPath and generate JSON path by template and use it to get value
                        string replaceIndicators = i.Replace("@@", "");
                        string pathJS = $"$.{replaceIndicators}";
                        JObject source = JObject.Parse(thisJson);
                        JToken value = source.SelectToken(pathJS);
                        //Consistently creating a new line 
                        stringResult += value.ToString();
                    }
                    else
                    {
                        if (i == "+" || i == "-" || i == "/" || i == "*")
                        {
                            stringResult += i;
                        }
                        else
                        {
                            stringResult += i;
                        }
                    }
                }
                //Using Evaluation to read string like math expression and get result
                var result = CSharpScript.EvaluateAsync(stringResult).Result;
                stringResult= result.ToString();
                Console.WriteLine($"String: {recordNumber} Computation Result: {stringResult}");
            }
            return stringResult;
        }
    }
}

//dotnet run output.txt --compute="@@title.IndexOf('W')"
//dotnet run output.txt --compute="@@title.Substring(1,4)"
//dotnet run output.txt --compute="@@title.Replace('W','H')"
//dotnet run output.txt --compute="@@title.StartsWith('H')"
//dotnet run output.txt --compute="@@quantityOnHand - @@quantityBackordered * @@quantityOnOrder"
//dotnet run output.txt --compute="@@quantityOnHand / @@quantityBackorder"



