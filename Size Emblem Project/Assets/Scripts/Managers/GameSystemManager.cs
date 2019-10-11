using SizeEmblem.Assets.Scripts.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Managers
{
    public class GameSystemManager : IGameSystemManager
    {
        #region Singleton Hackery

        protected GameSystemManager()
        {

        }

        private static GameSystemManager _instance;

        public static GameSystemManager Instance
        {
            get
            {
                if (_instance == null) _instance = new GameSystemManager();
                return _instance;
            }
        }

        #endregion



        public bool IsMouseInputEnabled { get; } = true;
        public bool IsControllerInputEnabled { get; } = true;
    }
}
