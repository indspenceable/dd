using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementShop : MonoBehaviour {
	private SessionManager session;
	[SerializeField] List<ShopIcon> partyInventorySlots;
	[SerializeField] List<ShopIcon> shopInventorySlots;
	private List<Item> shopInventory;

	[System.NonSerialized]
	public int currentPartyMember = -1;
	public void Setup(SessionManager session, List<Item> inventory) {
		this.session = session;
		this.shopInventory = inventory;
		foreach(var l in partyInventorySlots) {
			l.Init(session);
		}
		foreach(var r in shopInventorySlots) {
			r.Init(session);
		}
		RefreshView();
	}

	public void RefreshView() {
		for (int i = 0; i < 9; i+=1) {
			if (i < session.state.inventory.Count) {
				var item = session.state.inventory[i];
				var def = item.GetDef(session);
				partyInventorySlots[i].Set(def.Tooltip(ItemDefinition.ToolTipOptions.INCLUDE_SELL_COST), def.image, () => {
					session.state.money += def.SellCost();
					session.state.inventory.Remove(item);
					shopInventory.Add(item);
					RefreshView();
				});
			} else {
				partyInventorySlots[i].Clear();
			}
		}
		for (int i = 0; i < 9; i+=1) {
			if (i < shopInventory.Count) {
				var item = shopInventory[i];
				var def = item.GetDef(session);
				var cost = def.cost;
				shopInventorySlots[i].Set(def.Tooltip(ItemDefinition.ToolTipOptions.INCLUDE_BUY_COST), def.image, () => {
					if (session.state.money > cost) {
						session.state.money -= cost;
						shopInventory.Remove(item);
						session.state.inventory.Add(item);
						RefreshView();
					}
				});
			} else {
				shopInventorySlots[i].Clear();
			}
		}
	}

	public void ReturnToMap() {
		session.SwapToMapMode();
	}
}
