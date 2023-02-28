using System.Globalization;
using System.Xml;

namespace rt004
{
    internal static class Config
    {
        public static int Width { get; private set; } = 600;
        public static int Height { get; private set; } = 450;
        public static string OutputFilename { get; private set; } = "demo.pfm";
        public static Colorf BackgroundColor { get; private set; } = Colorf.BLACK;

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
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float v)) return v;
            else throw new FormatException($"Invalid float: {value}");
        }

        static Colorf ParseColor(XmlNode node)
        {
            if (node == null) throw new FormatException("Expected color node");
            if (node.Name != "color") throw new FormatException($"Expected color node, found {node.Name} instead");

            float r = 0, g = 0, b = 0;
            foreach (XmlNode child in node.ChildNodes)
            {
                float val = ParseFloat(child.InnerText);
                switch (child.Name)
                {
                    case "r": r = val; break;
                    case "g": g = val; break;
                    case "b": b = val; break;
                    default: throw new FormatException($"Invalid color parameter: {child.Name}");
                };
            }

            return new Colorf(r, g, b);
        }
    }
}
