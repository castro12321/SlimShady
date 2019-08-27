namespace SlimShadyEssentials.Monitors.Hardware.vcp
{
    public class Edid
    {
        public string raw { get; private set; }

        public string manufacturerId { get; private set; }
        public short productCode { get; private set; }
        public int serialNum { get; private set; }
        public byte weekOfManudacture { get; private set; }
        public byte yearOfManufacture { get; private set; }

        private Edid()
        {
        }

        public static Edid parse(string raw)
        {
            Edid edid = new Edid();
            edid.raw = raw;

            // ...

            return edid;
        }

    }
}
