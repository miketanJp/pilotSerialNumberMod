namespace it.miketan.PilotSerial
{
    public partial class ModLink : PhantomBrigade.Mods.ModLink
    {
        internal static int ModIndex;
        internal static string ModID;
        internal static string ModPath;

        public override void OnLoadStart()
        {
            ModIndex = modIndexPreload;
            ModID = modID;
            ModPath = metadata.path;

            //EnableHarmonyFileLog(); //Scommentare per eventuale debug della libreria; viene generato un file di testo sul desktop.
        }
    }
}