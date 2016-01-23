use std::io;
use std::io::Read;
use std::str::FromStr;
use std::cmp;

#[derive(Debug)]
struct Reindeer<'a> {
    name: &'a str,
    speed: usize,
    fly_time: usize,
    rest_time: usize
}

fn read_input() -> io::Result<String> {
    let mut buffer = String::new();
    try!(io::stdin().read_to_string(&mut buffer));
    Ok(buffer.trim().to_string())
}

fn parse_line(line: &str) -> Reindeer {
    let tokens: Vec<_> = line.split_whitespace().collect();
    Reindeer {
        name: tokens[0],
        speed: usize::from_str(tokens[3]).unwrap(),
        fly_time: usize::from_str(tokens[6]).unwrap(),
        rest_time: usize::from_str(tokens[13]).unwrap()
    }
}

fn distance_traveled(reindeer: &Reindeer, time: usize) -> usize {
    let cycle_time = reindeer.fly_time + reindeer.rest_time;
    let full_cycles = time / cycle_time;

    let mut traveled = reindeer.speed * reindeer.fly_time * full_cycles;

    let remaining_time = time % cycle_time;

    traveled += cmp::min(remaining_time, reindeer.fly_time) * reindeer.speed;

    traveled
}

fn main() {
    let input = read_input().unwrap();
    let deers: Vec<_> = input.lines().map(parse_line).collect();
    let time = 2503;

    let traveled: Vec<_> = deers
        .iter()
        .map(|deer| (deer.name, distance_traveled(deer, time)))
        .collect();

    for &(deer, distance) in &traveled {
        println!("{}: {} km", deer, distance);
    }

    let winner_distance = traveled.iter().map(|&(_, distance)| distance).fold(0, cmp::max);
    println!("Winner traveled {} km", winner_distance);
}
