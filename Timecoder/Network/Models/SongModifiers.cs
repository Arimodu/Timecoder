using Newtonsoft.Json;
using System;

namespace Timecoder.Network.Models
{
    [Serializable]
    public class SongModifiers
    {
        public bool NoFail { get; }
        public bool OneLife { get; }
        public bool FourLife { get; }
        public bool NoBombs { get; }
        public bool NoWalls { get; }
        public bool NoArrows { get; }
        public bool GhostNotes { get; }
        public bool DisappearingArrows { get; }
        public bool SmallNotes { get; }
        public bool ProMode { get; }
        public bool StrictAngles { get; }
        public bool ZenMode { get; }
        public string SongSpeed { get; }

        public SongModifiers(GameplayModifiers modifiers)
        {
            NoFail = modifiers.noFailOn0Energy;
            OneLife = modifiers.instaFail;
            FourLife = modifiers.energyType == GameplayModifiers.EnergyType.Battery;
            NoBombs = modifiers.noBombs;
            NoWalls = modifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles;
            NoArrows = modifiers.noArrows;
            GhostNotes = modifiers.ghostNotes;
            DisappearingArrows = modifiers.disappearingArrows;
            SmallNotes = modifiers.smallCubes;
            ProMode = modifiers.proMode;
            StrictAngles = modifiers.strictAngles;
            ZenMode = modifiers.zenMode;
            SongSpeed = modifiers.songSpeed.ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}