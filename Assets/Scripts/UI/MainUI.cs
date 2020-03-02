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
    public GameObject endTurnButton;
    public GameObject[] abilityButtons;
    public GameObject pickUpItemButton;

    private void Start() {
        Instance = this;
    }

    public void SetAbilityBar(Entity entity) {
        //Monster
        if (entity.allegiance == EntityAllegiance.monster) {
            HideAbilityBar();
        } else {
            CreatePlayerAbilityBar(entity);
        }
    }

    private void CreatePlayerAbilityBar(Entity entity) {
        //Player
        Ability[] abilities = entity.Interaction.abilities.ToArray();

        for (int i = 0; i < abilityButtons.Length; i++) {
            if (i < abilities.Length) {
                abilityButtons[i].SetActive(true);
                Text newButtonText = abilityButtons[i].GetComponentInChildren<Text>();
                newButtonText.text = abilities[i].name;
            } else {
                abilityButtons[i].SetActive(false);
            }
            CheckShowPickupButton();
            endTurnButton.SetActive(true);
        }
    }

    private void HideAbilityBar() {
        for (int i = 0; i < abilityButtons.Length; i++) {
            abilityButtons[i].SetActive(false);
        }
        pickUpItemButton.SetActive(false);
        endTurnButton.SetActive(false);
    }

    public void CheckShowPickupButton() {
        bool show = BattleController.Instance.currentEntity.Inventory.IsCollidingWithWorldItem();
        pickUpItemButton.SetActive(show);
    }

    public void Interact(int index) {
        //Obtain reference for ease of use
        Entity currentEntity = BattleController.Instance.currentEntity;

        //For Current entity, Change state to targeting.
        currentEntity.State = EntityState.targeting;
        currentEntity.Interaction.SetCurrentAbility(index);
    }

    public void PickUpItem() {
        //Obtain reference for ease of use
        Entity currentEntity = BattleController.Instance.currentEntity;

        currentEntity.Inventory.PickUpItemsOnFloor();
    }

    public void EndTurn() {
        BattleController.Instance.NextTurn();
    }
}
