namespace RestApi_SO.Models
{
    public class TagPercentage
    {
        public string Name;
        public double Percentage;

        public TagPercentage(string name, double percentage)
        {
            Name = name;
            Percentage = percentage;
        }
    }
}
