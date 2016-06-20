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
using System.Collections;
using System.Collections.Generic;
public class TaskManager{
	private IList updateTasks = new ArrayList();
	private IList fixedUpdateTasks = new ArrayList();
	private IList lateUpdateTasks = new ArrayList();

	public void addUpdateTask(Task task){
		this.updateTasks.Add(task);
	}

	public void addFixedUpdateTask(Task task){
		this.fixedUpdateTasks.Add(task);
	}

	public void addLateUpdateTask(Task task){
		this.lateUpdateTasks.Add(task);
	}

	public void runUpdateTasks(){
		for (int i = 0; i < updateTasks.Count; i++){
			Task task = (Task) updateTasks[i];
			if (task.execute()){
				updateTasks.Remove(task);
			}
		}
	}

	public void runFixedUpdateTasks(){
		for (int i = 0; i < fixedUpdateTasks.Count; i++){
			Task task = (Task) fixedUpdateTasks[i];
			if (task.execute()){
                fixedUpdateTasks.Remove(task);
			}
		}
	}

	public void runLateUpdateTasks(){
		for (int i = 0; i < lateUpdateTasks.Count; i++){
			Task task = (Task) lateUpdateTasks[i];

            try {
                if (task.execute()) {
                    lateUpdateTasks.Remove(task);
                }
            } catch (System.Exception e) {
                ModMain.instance.log(e);
                lateUpdateTasks.RemoveAt(i);
            }
		}
	}
}

