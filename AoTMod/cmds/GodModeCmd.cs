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

public class GodModeCmd : ICommand{

	public bool cmd (string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer("God mode: " + ModMain.instance.isGodMode());
			return true;
		}else if (args[0].Equals("on", StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.setGodMode(true);
			return true;
		}else if (args[0].Equals("off", StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.setGodMode(false);
			return true;
		}else{
			return false;
		}
	}
    public string getDescriptionString() {
        return "Toggles god mode on/off.";
    }

}

