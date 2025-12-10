# PILOT SERIAL MOD

**RELEASE DATE:** 2025/12/10  
**MOD VERSION:** 1.0.0  
**MOD LINK (Github):** [GitHub Repository](https://github.com/miketanJp/pilotSerialNumberMod)<br>
**PROGRAMMING LANGUAGE:** C-Sharp 7.3 (Framework v4.7.2)

---

## MOD INTRO

![demo](./demo.gif)<br>

This mod introduces a unique serial number system for **Phantom Brigade**, a game by **Brace Yourself Games**.<br>
The Pilot Customization screen uses the class ```CIViewBasePilotInfoExtended``` and its method ```RedrawForPilot``` for reflecting any changes made to the selected pilot (which is a `PersistentEntity` that needs to be found for the purpose).
This class is a derived view of Type ```CIView```, which takes care of every method and logic related to UI (although codebase has its own logic for the specific use case and custom UI elements that partially uses `NGUI`).
Once the user is making changes in the editor (the Bio, in this case), this will trigger the above-mentioned method, which is the target of the patching performed with **Harmony Framework**.

Now, when we select the customize Button for that specific pilot, this will trigger the method ```OnEditStart``` from the class ```CIViewBaseEditor```; it contains all the logic related to the customization screen, including the appearance and, naturally, the biography.
Among all the logic contained, the `bio` field for that `PersistentEntity` is cached (probably when writing in the input) before being saved and Serialized in the YAML file related to that pilot.
This change will make possible to see a randomly-generated S/N will be created and ready to be saved permanently.
The Serial Number is a sequence composed by two groups of 4 alphanumeric combo of [A-Z] (Upper-Case only) and [0-9]; once generated, even when entering in the edit screen again, the code number will be permanently assigned to that pilot and cannot be changed again.
This because the mod is checking if the following format that starts with PIL- is already present in the `bio` field (or already was before) for that specific pilot or not; if yes, it cannot be generated again.

Once generated, the serial number will be generated, already formatted with BBCode markup, with the following pattern:
```PIL-XXXX-XXXX```

A prior change in bio must be done the first time to see the result (as shown in the video abiove, which explains the use case).

### KEY FEATURES
- **Unique serial number** automatically generated for every pilot.
- **In-memory caching** to prevent regeneration during UI refreshes.
- Serial displayed directly in the **pilot’s bio**.
- Compatible with **both the editor and standard pilot view**.
- Safe: does not affect any other pilot stats or attributes.
- Safe to remove: the mod can be safely removed, significantly decreasing the chances to corrupt a save game.

---

## MOD AUTHOR
- .Miketan

## CREDITS
- Harmony Framework for the patching
- Phantom Brigade Modding System
- Brace Yourself Games for the awesome game!

---

## MOD STATUS
- **Steam Workshop:** 🟡  
  [Steam Workshop Link - TBA](#)
- **Nexus Mod:** 🟡  
  [Nexus Mod Link - TBA](#)

---

## INSTALLATION (EPIC GAME VERSION)
To install the mod:

1. Extract the mod folder into the following directory:
<br>```[Drive]:\Users\[yourUser]\AppData\Local\PhantomBrigade\Mods```
<br><br>
2. Launch the game; the mod will be automatically detected and activated.

> ⚠️ **[DISCLAIMER]** ⚠️
> <br>While the mod has been fully tested by covering most of the use cases, make sure to back up your save file before applying the mod to avoid any unintended (and negative) effects.
> <br><br>The mod author (.Miketan) will not be held responsible for any misuse of this mod or the damage can cause to saves corruption.
> <br>The above code project is made public to adhere with [Brace Yourself Games' guidelines](https://braceyourselfgames.com/mod-policy/)
> mostly to certify the present Library Code **DOES NOT CONTAIN** any malware and/or trojan in every form.
> <br><br>You are free to use/upload my mod as a dependency to other mods as long as you ask me permission and giving credits to me, as this mod is also covered under **BSD-3 Licence**.

---

## CHANGELOG
- Phantom Brigade 2.0 Support
- Initial Release