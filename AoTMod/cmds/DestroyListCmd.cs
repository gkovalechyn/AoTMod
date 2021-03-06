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
public class DestroyListCmd : ICommand{
	private string[] help = {
		"/destroyList",
		"/destroyList add <id>",
		"/destroyList rmv <id>"
	};
	public bool cmd (string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer(this.help);
			ModMain.instance.sendToPlayer("List: ");

			foreach (int id in ModMain.instance.getModMainThread().getDestroyList()){
				ModMain.instance.sendToPlayer("- " + id);
			}

			return true;
		}

		if (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getModMainThread().addToDestroyList(int.Parse(args[1]));
			return true;
		}

		if (args[0].Equals("rmv", StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getModMainThread().removeFromDestroyList(int.Parse(args[1]));
			return true;
		}

		return false;
	}

    public string getDescriptionString() {
        return "Controls the list of players who's gameobjects should be deleted.";
    }
}

