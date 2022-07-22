UNITY4 DYNAMIC FONT SUPPORT FOR NGUI
01/09/2013

DISCLAIMER:
This is a quick and dirty implementation of Unity 4 dynamic fonts in NGUI. It hasn't been fully tested and may have bugs.
USE AT YOUR OWN RISKS
This extension can only be used if you have a licence of NGUI with sources.
If you need more info, please contact UNISIP through private messages on the tasharen ngui forum or the unity3D forum.

HOW TO INSTALL
- create a new project
- import asset store NGUI package, latest version (2.7.7c)
- import NGUI_277c_DynamicFonts.unitypackage

HOW TO USE DYNAMIC FONTS
- to create a new NGUI font prefab:
	- GameObject/Create Empty, rename it MyDynamicFont for instance
	- attach component: UIFont
	- import a TTF file in the project
	- create material asset 'MyMaterial'
	- set MyMaterial shader to 'Unlit/Transparent Colored (DynamicFont)' (or a clipped variation if you need clipping)
	- in the inspector for 'MyDynamicFont', set font type to 'Dynamic'
	- drag the TTF font asset onto the Font field
	- drag MyMaterial onto the Material field
	- set the wanted font size and style
	- drag the gameObject from the Hierarchy to the Project view to make it a prefab
	- remove the gameObject from the Hierarchy
	- you can now use the newly created font prefab in your NGUI scene as a font for your UILabels
- for convenience, you may also duplicate an existing font prefab (drag into the hierarchy and then drag it back to project view, rename new prefab)
- you can then modify the font prefab to have it use a dynamic font
- if you need to use the same font, but with different style or size, you need to create several prefabs for that (each prefab acts as a 'style' in the sense of CSS style)

- when you change a font size, the change is not reflected on all UILabels. If you want to have your labels pixel perfect, you need to go into each of them and set their scale accordingly (that is, use the same value as font size)

KNOWN ISSUES:
- vertical pivot alignment seems a bit buggy
	
