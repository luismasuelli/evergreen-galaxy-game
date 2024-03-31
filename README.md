# A NetRose and EVMGames boilerplate for Unity

This is a boilerplate project. It includes EVMGames/NetRose and all the required dependencies, and will be tagged with local or specific versions of those dependency packages.

Unity version
-------------

The current status is meant to be used in Unity 2023.1.13f1 due to issues in previous versions.

Dependencies versioning and status
----------------------------------

So far this project relies on a set of packages that I could consider, at best, in beta status. These packages are not officially released and they are tagged 0.0.1 or alike (mostly in experimental status). The referenced packages must be cloned individually besides this boilerplate (i.e. as a sibling directory). They are:

 - com.alephvault.unity.support: [Clone it - 0.0.3](https://github.com/AlephVault/unity-support/tree/0.0.3).
 - com.alephvault.unity.support.generic: [Clone it - 0.0.1](https://github.com/AlephVault/unity-support-generic/tree/0.0.1).
 - com.alephvault.unity.boilerplates: [Clone it - 0.0.2](https://github.com/AlephVault/unity-boilerplates/tree/0.0.2).
 - com.alephvault.unity.layout: [Clone it - 0.0.1](https://github.com/AlephVault/unity-layout/tree/0.0.1).
 - com.alephvault.unity.menuactions: [Clone it - 0.0.2](https://github.com/AlephVault/unity-menu-actions/tree/0.0.2).
 - com.alephvault.unity.soundaround: [Clone it - 0.0.1](https://github.com/AlephVault/unity-soundaround/tree/0.0.1).
 - com.alephvault.unity.spriteutils: [Clone it - 0.0.4](https://github.com/AlephVault/unity-spriteutils/tree/0.0.4).
 - com.alephvault.unity.textureutils: [Clone it - 0.0.1](https://github.com/AlephVault/unity-textureutils/tree/0.0.1).
 - com.alephvault.unity.binary: [Clone it - 0.0.2](https://github.com/AlephVault/unity-binary/tree/0.0.2).
 - com.alephvault.unity.meetgard: [Clone it - 0.0.6](https://github.com/AlephVault/unity-meetgard/tree/0.0.6).
 - com.alephvault.unity.meetgard.auth: [Clone it - 0.0.5](https://github.com/AlephVault/unity-meetgard-auth/tree/0.0.5).
 - com.alephvault.unity.meetgard.scopes: [Clone it - 0.0.3](https://github.com/AlephVault/unity-meetgard-scopes/tree/0.0.3).
 - com.alephvault.unity.remotestorage: [Clone it - 0.0.2](https://github.com/AlephVault/unity-remotestorage/tree/0.0.2).
 - com.alephvault.unity.backpack: [Clone it - 0.0.2](https://github.com/AlephVault/unity-backpack/tree/0.0.2).
 - com.alephvault.unity.gabtab: [Clone it - 0.0.2](https://github.com/AlephVault/unity-gabtab/tree/0.0.2).
 - com.alephvault.unity.windrose: [Clone it - 0.0.8](https://github.com/AlephVault/unity-windrose/tree/0.0.8).
 - com.alephvault.unity.windrose.biomes: [Clone it - 0.0.4](https://github.com/AlephVault/unity-windrose-biomes/tree/0.0.4).
 - com.alephvault.unity.windrose-backpack: [Clone it - 0.0.2](https://github.com/AlephVault/unity-windrose-backpack-plugin/tree/0.0.2).
 - com.alephvault.unity.windrose-gabtab: [Clone it - 0.0.2](https://github.com/AlephVault/unity-windrose-gabtab-plugin/tree/0.0.2).
 - com.alephvault.unity.windrose.spriteutils: [Clone it - 0.0.4](https://github.com/AlephVault/unity-windrose-spriteutils/tree/0.0.4).
 - com.alephvault.unity.windrose.neighbourteleports: [Clone it - 0.0.3](https://github.com/AlephVault/unity-windrose-neighbourteleports/tree/0.0.3).
 - com.alephvault.unity.windrose.cubeworlds: [Clone it - 0.0.1](https://github.com/AlephVault/unity-windrose-cubeworlds/tree/0.0.1).
 - com.alephvault.unity.windrose.refmapchars: [Clone it - 0.0.4](https://github.com/AlephVault/unity-windrose-refmapchars/tree/0.0.4).
 - com.alephvault.unity.windrose.lpcbiomes: [Clone it - 0.0.1](https://github.com/AlephVault/unity-windrose-lpcbiomes/0.0.1).
 - com.alephvault.unity.netrose: [Clone it - 0.0.6](https://github.com/AlephVault/unity-netrose/tree/0.0.6).
 - com.alephvault.unity.netrose.storage: [Clone it - 0.0.2](https://github.com/AlephVault/unity-netrose-storage/tree/0.0.2).
 - com.alephvault.unity.netrose.refmapchars: [Clone it - 0.0.3](https://github.com/AlephVault/unity-netrose-refmapchars/tree/0.0.3).
 - com.alephvault.unity.ipfs: [Clone it - 0.0.1](https://github.com/AlephVault/unity-ipfs/tree/0.0.1).
 - com.alephvault.unity.deeplinks: [Clone it - 0.0.1](https://github.com/AlephVault/unity-deeplinks/tree/0.0.1).
 - com.alephvault.unity.evmgames.wallets: [Clone it - 0.0.1](https://github.com/AlephVault/unity-evmgames-wallets/tree/0.0.1).
 - com.alephvault.unity.evmgames.auth: [Clone it - 0.0.4](https://github.com/AlephVault/unity-evmgames-auth/tree/0.0.4).
 - com.alephvault.unity.evmgames.livecache: [Clone it - 0.0.3](https://github.com/AlephVault/unity-evmgames-livecache/tree/0.0.3).
 - com.alephvault.unity.evmgames.localstorage: [Clone it - 0.0.2](https://github.com/AlephVault/unity-evmgames-localstorage/tree/0.0.2).
 - com.alephvault.unity.evmgames.contracts: [Clone it - 0.0.2](https://github.com/AlephVault/unity-evmgames-contracts/tree/0.0.2).
 - com.alephvault.unity.evmgames.contracts.bridge: [Clone it - 0.0.2](https://github.com/AlephVault/unity-evmgames-bridge-contract/tree/0.0.2).
 - com.alephvault.unity.evmgames.deeplinks: [Clone it - 0.0.2](https://github.com/AlephVault/unity-evmgames-deeplinks/tree/0.0.2).
 - com.nethereum.unity: [Clone it - 4.19.2](https://github.com/Nethereum/Nethereum.Unity/tree/4.19.2).
 - com.nethereum.unity.walletconnect: [Clone it - 4.19.3](https://github.com/Nethereum/Nethereum.Unity.WalletConnect/tree/4.19.3).

You can run the attached scripts to clone the dependencies after cloning this project:

 - Windows: clone-all.bat
 - Unix-based (bash): clone-all.sh
 
If you have these repositories already cloned for another boilerplate and version, you can just go inside and checkout the proper (remote, typically) tags instead.

Licenses
--------

Pay attention to license notices in the readme for the following packages:

 - com.alephvault.unity.windrose.lpcbiomes
 - com.alephvault.unity.windrose.refmapchars

As a general rule of thumb, NEVER use any asset included in any of the Samples/ directory (in any package), since most of them were generated with tools that DO NOT allow free distribution.

