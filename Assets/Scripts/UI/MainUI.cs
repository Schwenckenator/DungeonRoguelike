using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class has methods which hook the code to the UI
/// It should not do any functions on its own.
/// </summary>
public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }
    public GameObject abilityButton;
    public Transform abilityPanel;
    public GameObject[] abilityButtons;

    private void Start() {
        Instance = this;
    }

    public void CreateAbilityBar(Entity entity) {
        Ability[] abilities = entity.Interaction.abilities.ToArray();

        for (int i=0; i< abilityButtons.Length; i++) {
            if(i < abilities.Length) {
                abilityButtons[i].SetActive(true);
                Text newButtonText = abilityButtons[i].GetComponentInChildren<Text>();
                newButtonText.text = abilities[i].name;
            } else {
                abilityButtons[i].SetActive(false);
            }

            
        }
    }

    public void Interact(int index) {
        //Obtain reference for ease of use
        Entity currentEntity = BattleController.Instance.currentEntity;

        //For Current entity, Change state to targeting.
        currentEntity.State = EntityState.targeting;
        currentEntity.Interaction.SetCurrentAbility(index);
        
    }


    public void EndTurn() {
        BattleController.Instance.NextTurn();
    }
}
