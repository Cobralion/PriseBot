using System;

namespace Infrastructure.Enities
{
    public record GuildSettings(string Channel, string Minimum, string Maximum)
    {

        public void Deconstruct(out int Minimum, out int Maximum)
        {
            Minimum = Convert.ToInt32(this.Minimum);
            Maximum = Convert.ToInt32(this.Maximum);
        }
    }
}