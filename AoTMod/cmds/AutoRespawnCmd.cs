//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.18444
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
public class AutoRespawnCmd : ICommand{

	public bool cmd (string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer("Auto respawn enabled: " + ModMain.instance.getModMainThread().autoRespawnEnabled);
			ModMain.instance.sendToPlayer("/autoRespawn <on|off>");
			return true;
		}else{
			ModMain.instance.getModMainThread().autoRespawnEnabled = args[0].Equals("on", StringComparison.OrdinalIgnoreCase);
			return true;
		}
	}

    public string getDescriptionString() {
        return "Toggles the auto-respawn.";
    }
}

