use std::cmp;

trait Alive {
    fn receive_damage(&mut self, damage: usize);
}

#[derive(Debug, Clone)]
struct Boss {
    health: usize,
    damage: usize
}

impl Alive for Boss {
    fn receive_damage(&mut self, damage: usize) {
        self.health -= std::cmp::min(damage, self.health);
    }
}

impl Boss {
    fn new() -> Boss {
        Boss { health: 58, damage: 9 }
    }
}

#[derive(Debug, Clone)]
struct Player {
    health: usize,
    armor: usize,
    mana: usize,
    active_effects: Vec<Effect>
}

impl Alive for Player {
    fn receive_damage(&mut self, damage: usize) {
        self.health -= std::cmp::min(damage, self.health);
    }
}

impl Player {
    fn new() -> Player {
        Player { health: 50, armor: 0, mana: 500, active_effects: Vec::new() }
    }
}

#[derive(Debug, Eq, PartialEq, Clone)]
enum Spell {
    MagicMissile,
    Drain,
    Shield,
    Poison,
    Recharge
}

#[derive(Clone)]
struct Castable {
    mana: usize,
    spell: Spell
}

impl Castable {

    fn all() -> Vec<Castable> {
        vec![
            Castable { mana: 53, spell: Spell::MagicMissile },
            Castable { mana: 73, spell: Spell::Drain },
            Castable { mana: 113, spell: Spell::Shield },
            Castable { mana: 173, spell: Spell::Poison },
            Castable { mana: 229, spell: Spell::Recharge },
        ]
    }

}

#[derive(Debug, Clone)]
struct Effect {
    timer: usize,
    spell: Spell
}

fn apply_effects(player: &mut Player, boss: &mut Boss) {
    for effect in player.active_effects.iter_mut() {
        effect.timer -=1;

        match effect.spell {
            Spell::Shield => player.armor = 7,
            Spell::Poison => boss.receive_damage(3),
            Spell::Recharge => player.mana += 101,
            _ => panic!("Spell {:?} cannot have effects", effect.spell)
        }
    }

    // remove expired effects
    for i in (0..player.active_effects.len()).rev() {
        if player.active_effects[i].timer == 0 {
            player.active_effects.remove(i);
        }
    }
}

fn cast(player: &mut Player, boss: &mut Boss, castable: &Castable) {
    player.mana -= castable.mana;
    match castable.spell {
        Spell::MagicMissile => boss.receive_damage(4),
        Spell::Drain => { player.health += 2; boss.receive_damage(2); }
        Spell::Shield => player.active_effects.push(Effect { timer: 6, spell: Spell::Shield }),
        Spell::Poison => player.active_effects.push(Effect { timer: 6, spell: Spell::Poison }),
        Spell::Recharge => player.active_effects.push(Effect { timer: 5, spell: Spell::Recharge })
    }
}

fn boss_turn(player: &mut Player, boss: &Boss) {
    let damage = if boss.damage > player.armor { boss.damage - player.armor } else { 1 };
    // println!("Boss attacks for {} damage", damage);
    player.receive_damage(damage);
}

fn pick_available_castables(player: &Player, castables: &Vec<Castable>) -> Vec<Castable> {
    castables
        .iter()
        .filter(|castable| player.mana >= castable.mana )
        .filter(|castable| player.active_effects.iter().all(|effect| effect.spell != castable.spell))
        .cloned()
        .collect()
}

fn round(player: Player, boss: Boss, castables: &Vec<Castable>, mana_spent_so_far: usize, best_result_so_far: usize) -> usize {
    if player.health <= 1 { best_result_so_far }
    // else if boss.health == 0 { cmp::min(best_result_so_far, mana_spent_so_far) }
    else {
        let mut player = player;
        let mut boss = boss;
        player.armor = 0;
        player.receive_damage(1);

        apply_effects(&mut player, &mut boss);

        if boss.health == 0 { cmp::min(best_result_so_far, mana_spent_so_far) }
        else {
            let available_castables = pick_available_castables(&mut player, castables);

            let mut best_so_far = best_result_so_far;

            for castable in available_castables {
                let mut player = player.clone();
                let mut boss = boss.clone();

                cast(&mut player, &mut boss, &castable);
                let mana_spent_so_far = mana_spent_so_far + castable.mana;
                if best_so_far <= mana_spent_so_far { continue; }

                player.armor = 0;
                apply_effects(&mut player, &mut boss);
                if boss.health > 0 {
                    boss_turn(&mut player, &mut boss);
                    best_so_far = round(player, boss, castables, mana_spent_so_far, best_so_far);
                } else {
                    best_so_far = mana_spent_so_far;
                }
            }

            // println!("Best result so far: {}", best_so_far );
            best_so_far
        }
    }
}

fn main() {
    let castables = Castable::all();
    let player = Player::new();
    let boss = Boss::new();

    let mana_spent = round(player, boss, &castables, 0, usize::max_value());
    println!("Min amount of mana to win: {:?}", mana_spent);
}
