using System.Xml;

namespace rt004
{
    internal static class Config
    {
        public static int Width { get; private set; } = 600;
        public static int Height { get; private set; } = 450;
        public static string OutputFilename { get; private set; } = "demo.pfm";
        public static float[] BackgroundColor { get; private set; } = new float[] { 0, 0, 0 };

        // TODO: camera options, color01/color255, shapes, lights

        public static void Load(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                ProcessNode(node);
            }
        }

        static void ProcessNode(XmlNode node)
        {
            string parameter = node.Name;

            switch (parameter)
            {
                case "width":
                    Width = ParseInt(node.InnerText);
                    return;
                case "height":
                    Height = ParseInt(node.InnerText);
                    return;
                case "output":
                    OutputFilename = node.InnerText;
                    return;
                case "background":
                    BackgroundColor = ParseColor(node.FirstChild);
                    return;
            }

            throw new FormatException($"Invalid parameter: {parameter}");
        }

        static int ParseInt(string value)
        {
            if (int.TryParse(value, out int v)) return v;
            else throw new FormatException($"Invalid int: {value}");
        }

        static float ParseFloat(string value)
        {
            if (float.TryParse(value, out float v)) return v;
            else throw new FormatException($"Invalid float: {value}");
        }

        static float[] ParseColor(XmlNode node)
        {
            if (node == null) throw new FormatException("Expected color node");
            if (node.Name != "color") throw new FormatException($"Expected color node, found {node.Name} instead");

            float[] output = new float[3];
            foreach (XmlNode child in node.ChildNodes)
            {
                int index = child.Name switch
                {
                    "r" => 0,
                    "g" => 1,
                    "b" => 2,
                    _ => -1
                };

                if (index == -1) throw new FormatException($"Invalid color parameter: {child.Name}");

                output[index] = ParseFloat(child.InnerText);
            }

            return output;
        }
    }
}
