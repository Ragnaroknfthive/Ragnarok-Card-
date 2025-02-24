# Ragnarok: A Mythological NFT-Based P2E Strategy Game

## Game Design Document

### 1. Introduction
Welcome to Ragnarok - an NFT-based, Play-to-Earn (P2E) strategy game steeped in mythological themes and designed around skillful, strategic combat. The following document outlines all key aspects of Ragnarok’s design, from its overarching storyline and core gameplay loops to its unique mix of Chess, turn-based duels, and Poker-inspired mechanics.  
This GDD will answer most of your questions.

### 2. Overview and Elevator Pitch

#### Game Concept
- **Genre/Setting:** A strategic, mythologically themed deck-builder that merges Chess, turn-based duels, and Poker-like mechanics.
- **NFT Integration:** All gods, titans, demigods, and legendary beings are tradable NFTs, each capable of leveling up and recording combat history permanently.
- **P2E Economy:** Players can earn, stake, or spend in-game currency (and HBD) to upgrade their characters and obtain powerful items, spells, or new gods.

#### Elevator Pitch
In Ragnarok, you command a mythological Chess army; your pieces are gods, titans, or legendary beings. When two pieces meet on the board, they engage in deep, 1v1 turn-based battles. Victory relies on strategic planning, skillful resource management, and a little “favor from the gods” through a Poker-inspired combat phase. Once a year, there is a year-long battle to claim glory. The battle resets yearly in a grand mythological war, allowing new deities from varied pantheons to join the fray. Outwit, outlast, and outplay your opponents to claim immortal glory and reap the spoils of war.

### 3. Setting, Story, and Key Themes

#### Mythological Inspiration
- **Initial Pantheon:** Norse mythology forms the starting roster, featuring deities like Odin, Thor, Freya, and more.
- **Future Pantheons:** Each year, additional gods, titans, and legendary beings from other mythologies (e.g., Greek, Egyptian, Hindu, etc.) will be added, obtainable via in-game rewards. 

#### Story Synopsis
Ragnarok, the end of the world, is here. All mythologies converge in an endless cycle of annual wars. From gods to mortals, each must choose sides and fight for glory, loot, and influence.  
- **Yearly Reset:** At the close of every war, victors earn bountiful rewards, and new deities join the battlefield. History repeats itself, yet the legends of each year are forever recorded on the blockchain.

#### Themes
- **Eternal Struggle:** Life, death, and rebirth form the central loop. Each war is an opportunity for immortality.
- **Skill Over Stats:** While stats matter, creative gameplay and skillful execution can trump numeric disadvantages.
- **Choice & Consequence:** Every battle is unique. Each decision in Chess, in spells/pets used, and in the Poker-style combat phase can permanently change the outcome of your war efforts.

### 4. Key Features
- **Strategic Chess Foundation:** The board and piece movements are derived from classic Chess, but each piece represents a mythological being with unique spells and stats.
- **Deep 1v1 Turn-Based Combat:** When two pieces meet, they engage in a complex battle involving spells, pets, and a Poker-inspired attack phase.
- **Extensive Character Customization:** NFT gods can be leveled up from Level 1 to Level 9, unlocking spells, passive abilities, weapons, and equipment.
- **Permanent Battle History:** Each NFT’s combat record is stored immutably, showcasing their exploits, victories, and defeats.
- **Diverse Mythologies:** New pantheons release annually, expanding the roster with new gods, titans, and legendary beings.
- **Play-to-Earn (P2E) Economy:**  
    - **HBD sink:** All HBD spent on in-game items goes into a SIP v1 where interest is redistributed to players.  
    - **Earn EXP coins** from battles to upgrade your gods.
- **Skill-Based Gameplay:** The game prides itself on depth and complexity; lesser stats can still prevail if paired with superior tactics.

### 5. Gameplay Overview

#### 5.1. Core Gameplay Loop
1. **Chess Phase:**  
    - Move your mythological Chess pieces according to classic Chess rules (customized for a 10-piece setup).
    - Each move grants +1 Stamina to every surviving being on your side.
2. **Initiate 1v1 Combat:**  
    - When two opposing pieces land on the same square, combat begins.  
    - Combat has two phases: Spell/Pet Phase → Attack Phase.
3. **Post-Battle Resolution:**  
    - The loser’s piece is removed from the Chessboard.  
    - The winner’s piece retains its remaining health and resources (minus used spells, pets, etc.).
4. **Return to Chess Phase:**  
    - Continue until one side’s King is defeated.

#### 5.2. Chess Phase Details

- **Initial Setup:**  
    - Each player starts with 10 pieces: 1 King, 1 Queen, 1 Bishop, 1 Knight, 1 Rook, and 5 Pawns.
    - Each piece is a different “class” with unique movement, spells, and roles.

- **Elements on the Board:**  
    - Some squares may have elemental properties (Fire, Water, Electricity, Earth).
    - Certain elementals are weak/strong against others (e.g., Fire < Water, Water > Fire).

- **Movement Rules:**  
    - Standard Chess movement (e.g., Rook moves in straight lines, Knight in L-shapes, etc.), but fewer pieces.  
    - Each completed turn grants +1 Stamina to every surviving being on that player’s side.

#### 5.3. Combat Mode
When two pieces occupy the same square, a 1v1 battle ensues. This battle comprises:

1. **Spell & Pet Phase**
2. **Attack Phase (Poker-Inspired)**

**Resources:**
- **Health Pool (HP):** Persists across multiple fights unless fully depleted.
- **Stamina Pool (STA):** Used to perform actions (e.g., large attacks cost Stamina).

##### 5.3.1. Spell & Pet Phase
- **Spells:**  
    Each god has a specific pool of spells (22 for Pawns, 30 for standard gods/titans, 33 for Queens, 35 for Kings).  
    Each spell has a mana cost (1–9) and a level (1–9, indicating power).  
    Players start each 1v1 with 1 mana, and gain +1 mana each turn until they reach 9 mana maximum.

- **Pets:**  
    Pets are special, summonable entities with their own HP, Speed, and Attack stats.  
    Pets can have passive or active abilities, and they can be leveled from 1–9.

##### 5.3.2. Attack Phase (Poker-Inspired Mechanics)
Once spells and pets have resolved, both players’ gods move into a direct combat phase driven by a No-Limit Hold ‘em poker analogy:
- **Bet → Attack**
- **Reraise → Counter Attack**
- **Call → Engage**
- **Fold → Brace (with a small health penalty)**
- **Check → Defend**
- **Chips → Hit Points (HP)**
- **Flop / Turn / River → Faith / Foresight / Destiny**

### 6. Progression and Customization
- **Experience (EXP Coins):**  
    Earned after every match.  
    Stake EXP coins to level up your gods (Levels 1–9). Higher levels unlock more spells, better stats, and advanced passives.

- **Spell Management:**  
    You can equip up to 30 spells on a standard god/titan (35 for a King).

- **Equipment & Items:**  
    Weapons, armor, and other gear can augment a god’s stats, grant special abilities, or buff existing spells.

- **NFT Ownership:**  
    Each god or legendary being is a unique NFT with a recorded history. You can trade, buy, or sell them on supported marketplaces.

### 7. Monetization and NFT Integration

- **Play-to-Earn Model:**  
    Players earn HBD or EXP coins through consistent play and successful battles.  
    A portion of the game’s revenue goes into a SIP v1, with interest paid out to players periodically.

- **HBD Sink:**  
    In-game transactions for items, upgrades, or minted gods use HBD, which is then locked in the SIP v1.

### 8. Look and Feel
- **Art Style:**  
    Fantasy/Mythological aesthetic featuring vibrant elemental effects, iconic godly attire, and stylized medieval weaponry.

- **Audio:**  
    Epic, orchestral soundtrack with dynamic intensity during chess moves and combat phases.

- **User Interface / Controls:**  
    Primarily mouse-based interaction for Chess, spells, and combat choices.

- **Accessibility:**  
    Clear, readable fonts and iconography to help new players recognize different gods, spells, or phases.  
    Color-blind friendly design for elemental squares or card suits.

### 9. Future Plans

- **Beta & Balancing:**  
    Collect user feedback to refine gameplay balance, especially synergy between Chess movement and Poker-based combat.

- **Seasonal Content & Expansions:**  
    Introduce new pantheons annually.  
    Seasonal events with limited-time spells, pets, or NFT drops.

- **Community Engagement:**  
    Regular AMAs, patch notes, dev diaries, and lore expansions to keep the community informed and invested.

### 10. Conclusion
Ragnarok aims to combine the timeless strategy of Chess, the deep tactical nature of turn-based RPGs, and the psychological drama of No-Limit Hold ’em Poker into one cohesive and thrilling experience. By leveraging NFTs for ownership and a P2E structure, Ragnarok rewards its most dedicated and skillful players both in and out of the game.  
Join us as we shape this epic mythological battleground—test your creativity, hone your tactics, and prepare for the annual cycle of world-shattering combat. This GDD lays the foundation, but Ragnarok, like any living myth, will continue to evolve.

### Appendix / Quick Reference

**Chess Terminology:**  
- 10 total pieces (1 King, 1 Queen, 1 Bishop, 1 Knight, 1 Rook, 5 Pawns)

**Combat Terminology:**  
- **Spell Phase →** Summon pets, cast spells, or skip.  
- **Attack Phase →** Poker-like system of attacking (bet) or bracing (fold).

**Stats and Resources:**  
- **HP →** Health Points  
- **STA →** Stamina (max 10 in duels, recovers between fights)  
- **Mana →** Up to 9 in the Spell Phase, replenishes each round.  
- **Speed Points →** Earned by large HP attacks; spent to chain attacks.  
- **Rage →** Earned by consecutive attacks in the same range (Light, Medium, Heavy).  

**Yearly Cycle:**  
- New mythologies, new NFTs, new expansions.
