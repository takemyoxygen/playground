use std::io;
use std::io::Read;
use std::str::FromStr;
use std::collections::{HashMap, HashSet};

type Person = String;
type HappinessTable = HashMap<(Person, Person), i32>;

fn read_input() -> io::Result<String> {
    let mut buffer = String::new();
    try!(io::stdin().read_to_string(&mut buffer));
    Ok(buffer.trim().to_string())
}

fn parse_input(input: &str) -> Vec<(&str, i32, &str)> {
    input
        .lines()
        .map(|line| {
            let tokens: Vec<_> = line.split_whitespace().collect();
            let name = tokens[0];
            let neigbour = tokens[10].trim_right_matches('.');
            let happiness = i32::from_str(tokens[3]).unwrap();
            let multiplier = if tokens[2] == "gain" { 1 } else { -1 };
            (name, happiness * multiplier, neigbour)
        })
        .collect()
}

fn add(name1: &str, name2: &str, happiness: i32, table: &mut HappinessTable){
    let mut entry = table.entry((name1.to_string(), name2.to_string())).or_insert(0);
    *entry += happiness;
}

fn build_happiness_table<'a>(rules: &'a Vec<(&str, i32, &str)>) -> HappinessTable {
    let mut table = HappinessTable::new();

    for &(name, happiness, neighbour) in rules {
        add(name, neighbour, happiness, &mut table);
        add(neighbour, name, happiness, &mut table);
    }

    table
}

fn find_path(
    person: &Person,
    remaining: &HashSet<&Person>,
    happiness_table: &HappinessTable,
    happiness_so_far: i32) -> (Person, i32) {
    if remaining.len() == 1 {
        (person.clone(), happiness_so_far)
    } else {
        let mut remaining = remaining.clone();
        remaining.remove(person);
        remaining
            .iter()
            .map(|&next| {
                let happiness_so_far = happiness_so_far + happiness_table.get(&(person.clone(), next.clone())).unwrap();
                let c = remaining.clone();
                find_path(next, &c, happiness_table, happiness_so_far)
            })
            .fold(("n/a".to_string(), i32::min_value()), |(best_person, best_happiness), (person, happiness)|{
                if best_happiness > happiness { (best_person, best_happiness) }
                else { (person, happiness) }
            })
    }
}

fn max_happiness(happiness_table: &HappinessTable) -> i32 {
    let mut persons = HashSet::new();

    for &(ref person, _) in happiness_table.keys() {
        persons.insert(person);
    }

    let start = persons.iter().next().unwrap().clone();

    persons.remove(start);

    let (last, happiness) = persons
        .iter()
        .map(|&next| {
            let happiness_so_far = happiness_table.get(&(start.clone(), next.clone())).unwrap();
            let c = persons.clone();
            find_path(&next, &c, happiness_table, *happiness_so_far)
        })
        .fold(("n/a".to_string(), i32::min_value()), |(best_person, best_happiness), (person, happiness)|{
            if best_happiness > happiness { (best_person, best_happiness) }
            else { (person, happiness) }
        });

    happiness + happiness_table.get(&(start.clone(), last)).unwrap()
}

fn add_myself(happiness_table: &mut HappinessTable) {
    let persons: Vec<_> = happiness_table.keys().map(|&(ref p, _)| p).cloned().collect();

    for person in persons {
        add("me", &person, 0, happiness_table);
        add(&person, "me", 0, happiness_table);
    }
}

fn main() {
    let input = read_input().unwrap();
    let happiness_rules = parse_input(&input);

    println!("Happiness Rules:");
    for rule in &happiness_rules {
        println!("{:?}", rule);
    }

    let mut table = build_happiness_table(&happiness_rules);
    println!("Happiness table: {:?}", table);
    println!("Total rows in the table {}", table.len());

    let happiness = max_happiness(&table);
    println!("Max happiness: {}", happiness);

    add_myself(&mut table);
    let happiness = max_happiness(&table);
    println!("Max happiness with myself: {}", happiness);
}
