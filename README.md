# World Simulation App
**VIRTUALIZED** Global Geopolitical and Economic Simulation Platform.

*"But there are times it is sad, in the long dream. It creates worlds that have no summer, and it shivers under a black sun, and it takes its sad creation for reality." — End Poem*

<img width="1920" height="1440" alt="313_1x_shots_so" src="https://github.com/user-attachments/assets/ee8ec72c-b947-467a-951d-de704132f69d" />


## Project Overview

WorldSimApp is a sophisticated desktop application designed for simulating and analyzing complex geopolitical, economic, and social dynamics on a global scale. Built with modern .NET 8 technology and the Avalonia cross-platform UI framework, this simulation engine provides an immersive platform for exploring the intricate relationships between nations, economies, military forces, and societal structures.

The primary purpose of WorldSimApp is to offer users a comprehensive tool for understanding how various factors interact and influence world events over time. Through advanced algorithmic modeling, the simulation generates realistic outcomes based on thousands of interconnected variables, allowing users to observe emergent behaviors and historical patterns that mirror real-world complexities.

The simulation operates on a turn-based system where each turn represents a time period in which all nations make decisions, experience events, and undergo changes across multiple domains. The engine manages interactions between countries, tracks historical developments, and maintains comprehensive statistics that users can analyze to gain insights into global trends and power dynamics.

## Installation

To install WorldSimApp on your local machine, simply download the latest release, extract it, and enjoy!

## Highlights

WorldSimApp offers several distinctive features that set it apart from conventional simulation applications:

**Comprehensive Economic Modeling**: The simulation incorporates sophisticated economic systems that model GDP growth through multiple factors including capital accumulation, labor force dynamics, total factor productivity, and technological advancement. The system implements the Taylor Rule for monetary policy simulation, Phillips Curve relationships between unemployment and inflation, and detailed trade balance calculations that account for exchange rate effects, tariffs, and sanctions.

**Dynamic Political Systems**: Each nation in the simulation maintains a complex political structure including ruling parties, policy agendas, public opinion tracking, and election cycles. Political changes ripple through societies, affecting stability, happiness, and economic performance. The system models ideological shifts, policy implementations with time lags, and political crises that can destabilize governments.

**Advanced Military Simulation**: Military forces are modeled with realistic branch structures including army, navy, and air power. The war simulation system considers terrain advantages, supply difficulties, morale factors, and alliance contributions. Wars impact economies, stability, and populations, with realistic casualty calculations and war fatigue accumulation over extended conflicts.

**Diplomatic Relations Network**: The simulation maintains a comprehensive diplomatic relationship system tracking alliances, enmities, trade agreements, and treaty obligations. International organizations such as the Global Alliance Council, Atlantic Defense Pact, Continental Union, Energy Cartel, and World Trade Organization influence global events and provide frameworks for international cooperation or conflict.

**Event-Driven Narrative**: The simulation generates a rich tapestry of events including economic crises, natural disasters, technological breakthroughs, political scandals, and diplomatic initiatives. Country-specific events trigger based on national conditions, while chain events create cascading effects across multiple systems. The global economic cycle moves through phases of recovery, expansion, peak, and recession, affecting all nations simultaneously.

**Historical Memory and Path Dependence**: Countries maintain memory of past conflicts, diplomatic interactions, and grievances that influence future decisions. This creates path-dependent behaviors where historical events continue to shape relationships decades later, mirroring real-world geopolitical dynamics.

**Social Demographics Modeling**: Population systems track age distributions, fertility rates, migration patterns, urbanization, education levels, healthcare quality, and social mobility. Demographic changes unfold over many simulation turns, with aging populations creating pension pressures and youth bulges potentially creating instability.

## Features

### Core Simulation Engine

The Core Simulation Engine serves as the central orchestrator managing all simulation subsystems. It maintains the collection of countries, events, wars, organizations, and resources while coordinating the sequence of operations that occur each turn. The engine initializes default values for each country based on their development level, creates historical tracking for GDP and stability metrics, and manages save/load functionality through the GameState system.

Each turn executes a carefully ordered sequence of simulations: economic changes first, followed by political developments, military updates, diplomatic relations, trade flows, war resolution, social changes, AI decision-making, and finally event generation. This ordering ensures that economic conditions inform political actions, military capabilities influence diplomatic options, and social conditions affect all other systems.

### Economic System

The Economic System implements sophisticated macroeconomic modeling that goes far beyond simple growth calculations. The system maintains market data for major resource categories including oil, natural gas, coal, iron, gold, food, technology, and weaponry. Each resource has supply, demand, price volatility, and base price characteristics that fluctuate based on global conditions.

The Taylor Rule monetary policy implementation adjusts interest rates based on inflation gaps and output gaps, with central banks in different countries responding to economic conditions according to their policy frameworks. The Phillips Curve simulation models the short-run tradeoff between unemployment and inflation, with NAIRU (Non-Accelerating Inflation Rate of Unemployment) calculations that vary based on education levels and economic growth.

Industry chain modeling captures interdependencies between sectors, recognizing that technology industries depend on semiconductor supply chains, energy industries require petroleum inputs, and agricultural sectors need favorable conditions. Supply and demand interactions affect prices, which then influence production decisions and economic growth.

The economic growth calculation blends potential growth (based on capital accumulation, labor force growth, and productivity improvements) with actual growth (driven by consumption, investment, government spending, and net exports). The blend weight depends on the output gap, with developing economies growing closer to potential while developed economies operate closer to capacity constraints.

### Political System

The Political System manages the complex political dynamics within each country. Elections occur according to constitutional cycle requirements, with vote shares influenced by economic performance, stability, scandals, and random factors. When governments change, policy reversals create transition costs as new administrations undo or modify previous policies.

Public opinion evolves based on economic conditions, inflation, unemployment, and media influence. Issue salience tracks which problems citizens consider most important, potentially triggering policy responses when issues become salient. Government approval ratings reflect overall performance while approval trends indicate trajectory.

The system models political spectrum positions from far-left to far-right, with different ideological orientations influencing policy priorities. Left-leaning governments tend toward welfare policies and higher taxation, while right-leaning governments prioritize military spending and market-oriented policies. Coalition governments and minority administrations create additional complexity.

Political crises can emerge from scandals, stability problems, or constitutional challenges. These events damage government approval, reduce stability, and may trigger early elections or leadership changes. The system captures the unpredictable nature of politics while maintaining reasonable bounds on political volatility.

### Military System

The Military System models defense forces with attention to branch composition, spending levels, readiness, and industrial base. Military branches (army, navy, air force) receive allocations based on geographic circumstances, with island nations emphasizing naval power and continental powers maintaining larger ground forces.

Military spending consumes economic resources but provides security benefits and industry spillovers. The system models defense industry output based on manufacturing capacity and technology levels, recognizing that nations with robust industrial bases can sustain longer wars than those dependent on imports.

War fatigue accumulates during extended conflicts, reducing military effectiveness and civilian support. Military readiness depends on training, equipment quality, morale, and absence of corruption. Wars cause GDP damage, stability reductions, population displacement, and long-term grievances that affect future diplomatic relations.

### Diplomatic System

The Diplomatic System maintains bilateral relationships between all pairs of countries, tracking relationship scores that range from -100 (hostile) to +100 (allied). Relationships evolve based on trade ties, cultural similarities, religious affiliations, historical conflicts, and recent diplomatic actions.

Sanctions represent a key diplomatic tool, with the system modeling trade embargoes, financial restrictions, arms embargoes, travel bans, and diplomatic sanctions. Sanctions damage target economies while creating diplomatic friction, and international organizations may respond to sanction decisions with their own actions.

Alliance systems provide mutual defense commitments and collective security frameworks. The simulation models NATO-style defense pacts where attacks on one member may trigger responses from all members. Alliance values decay when relationships deteriorate and grow through cooperation.

International organizations influence global affairs through peacekeeping missions, economic cooperation, trade negotiations, and regulatory frameworks. Organizations maintain member rolls, budgets, and action capabilities that affect their effectiveness.

### War System

The War System Models warfare with attention to operational realities. Terrain affects defensive advantages, with mountainous regions, jungles, deserts, and urban environments each providing different military characteristics. Supply difficulties model the logistical challenges of projecting power at distance.

Alliance involvement can escalate local conflicts into broader wars as allied nations honor defense commitments. The system tracks war progression through multiple turns, with battle outcomes influenced by military power, morale, technology, and terrain. Casualties accumulate based on combat intensity and duration.

Wars end through victory, defeat, negotiated peace, or stalemate. Victory may extract reparations from the defeated party while defeat damages the victor's economy through war exhaustion. Stalemates produce ceasefires with territorial status quo preservation. Post-war recovery takes many turns as societies heal and economies rebuild.

### Social System

The Social System models demographic transitions that unfold over decades of simulation time. Population pyramids evolve based on fertility rates, mortality rates, and migration. Aging populations create pension pressures and labor force challenges while youth bulges can create either opportunity or instability.

Migration flows respond to economic opportunities, safety conditions, and quality of life factors. Nations with strong economies and stable societies attract immigrants while troubled nations experience brain drain and population loss. Urbanization increases as rural populations seek better opportunities in cities.

Education systems improve over time based on government spending, economic conditions, and political stability. Literacy rates, tertiary enrollment, and STEM graduate rates all evolve based on national characteristics. Better education increases productivity, reduces crime, and improves health outcomes.

Healthcare quality correlates with economic development and government investment. Life expectancy, infant mortality, and healthcare costs all respond to systemic conditions. Aging populations increase healthcare costs while improvements in healthcare contribute to longer, more productive lives.

Crime rates respond to economic conditions, education levels, inequality, and enforcement spending. High crime reduces stability and happiness while consuming resources for law enforcement. Social mobility reflects opportunities for advancement based on education, corruption levels, and economic conditions.

### AI Behavior System

The AI Behavior System controls all non-player countries, making decisions that advance national interests within constraints. Strategic goals establish primary and secondary objectives that guide decision-making, with goals updating periodically based on changing circumstances.

Countries pursue various strategic objectives including economic growth, economic recovery, stabilization, regional hegemony, regional influence, alliance building, technological advancement, trade surplus, military modernization, and education reform. The primary objective responds to current conditions, with struggling economies prioritizing recovery and powerful nations seeking regional dominance.

Geopolitical evaluations identify regional rivals and opportunities for influence expansion. Resource competition creates diplomatic tensions over strategic materials. Alliance strategies evaluate potential partners and maintain existing relationships. Historical memory influences current decisions, with past conflicts creating lasting suspicions.

Military strategies adjust spending based on threat levels and strategic objectives. Countries seeking regional hegemony invest heavily in military capabilities while those focusing on development maintain minimal deterrence. The AI responds to regional threats by increasing defense spending when neighboring rivals pose dangers.

### Event System

The Event System generates the historical narrative that makes each simulation unique. Global economic cycles move through phases, creating broader conditions that affect all nations. Recessions reduce growth worldwide while expansions create opportunities for prosperity.

Financial crises can strike randomly, causing market crashes or banking failures that damage economies. Chain events create cascading effects where initial shocks trigger secondary consequences. Technology breakthroughs transform economies as innovations spread from early adopters to laggards.

Country-specific events capture unique national circumstances. The United States might experience tech booms, housing bubbles, or trade wars. China might undergo economic reforms, demographic challenges, or technological competition. Germany might face reunification effects, euro crises, or automotive industry disruptions. These events trigger based on national conditions and create historically plausible developments.

Random events across categories including economic, military, political, social, diplomatic, and natural disaster types add variety to each simulation. The frequency and intensity of events respond to simulation settings, allowing users to configure how turbulent their historical narrative becomes.

## License

**Copyright © 2026 Sean Hank.**  
All rights reserved.

See: `LICENSE`

## Credits

See: `CREDITS.md`

## Contributing

This repository is currently NOT accepting public contributions.

Forks and private modifications are not supported through official channels. Users who wish to create derivative works may do so under the constraints of the copyright license, though no assistance or guidance for such modifications is provided.

Questions, bug reports, and feature requests will not receive responses through public channels. The project maintains its current scope and direction without external input.
