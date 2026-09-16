using System;
using System.Collections.Generic;

namespace Ko.StageSelect
{
    [Serializable]
    public sealed class StageProgressSaveData
    {
        /// <summary>
        /// セーブデータの形式のバージョン。将来的に形式が変わった場合に使用する
        /// </summary>
        public int Version = 1;
        public List<string> clearedStageIds = new();
    }
}
