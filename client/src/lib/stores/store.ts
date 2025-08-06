import { createContext } from "react";
import CounterStore from "./counterStore";
import { UiStore } from "./UiStore";

interface Store{
    counterStore: CounterStore
    uiStore: UiStore
}

//You're just creating an object (store) that holds an instance of CounterStore and UiStore.
export const store: Store = {
    counterStore: new CounterStore(),
    uiStore: new UiStore()
}

export const StoreContext = createContext(store);