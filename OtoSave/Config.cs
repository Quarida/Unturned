using System;
using Rocket.API;

namespace OtoSave
{
	public class Config : IRocketPluginConfiguration, IDefaultable
	{
		public void LoadDefaults()
		{
			this.DosyaYolu = "DosyaYolu";
			this.Süre = 10;
		}

		public string DosyaYolu;
		public int Süre;
	}
}