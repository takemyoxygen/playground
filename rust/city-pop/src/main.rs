extern crate getopts;
extern crate rustc_serialize;
extern crate csv;

use std::env;
use getopts::Options;
use std::path::Path;
use std::fs::File;
use std::error::Error;
use std::io;
use std::fmt;

#[derive(Debug, RustcDecodable)]
struct Row {
    country: String,
    city: String,
    accent_city: String,
    region: String,
    population: Option<u64>,
    latitude: Option<f64>,
    longitude: Option<f64>,
}

#[derive(Debug)]
struct Population {
    country: String,
    city: String,
    population: u64
}

#[derive(Debug)]
enum CliError {
    Args(String),
    Io(io::Error),
    Csv(csv::Error),
    NotFound(String)
}

impl fmt::Display for CliError {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match *self {
            CliError::Args(ref reason) => write!(f, "There's a problem with command-line arguments: {}", reason),
            CliError::Io(ref err) => err.fmt(f),
            CliError::Csv(ref err) => err.fmt(f),
            CliError::NotFound(ref city) => write!(f, "No matching cities with name {} with a population were found.", city),
        }
    }
}

impl Error for CliError {
    fn description(&self) -> &str {
        match *self {
            CliError::Args(ref reason) => reason,
            CliError::Io(ref err) => err.description(),
            CliError::Csv(ref err) => err.description(),
            CliError::NotFound(_) => "Population not found",
        }
    }
}

impl From<io::Error> for CliError {
    fn from(err: io::Error) -> CliError {
        CliError::Io(err)
    }
}

impl From<csv::Error> for CliError {
    fn from(err: csv::Error) -> CliError {
        CliError::Csv(err)
    }
}

impl From<getopts::Fail> for CliError {
    fn from(err: getopts::Fail) -> CliError {
        CliError::Args(std::error::Error::description(&err).to_string())
    }
}

impl Population {
    fn new(country: String, city: String, population: u64) -> Population {
        Population{country: country, city: city, population: population}
    }
}

#[derive(Debug)]
enum DataSource {
    File(String),
    Stdin
}

#[derive(Debug)]
struct AppOptions {
    source: DataSource,
    city: String,
    quiet: bool
}

impl AppOptions {
    fn new(file: Option<String>, city: String, quiet: bool) -> AppOptions {
        let input_type = match file {
            Some(file) => DataSource::File(file),
            None => DataSource::Stdin
        };

        AppOptions { source: input_type, city: city, quiet: quiet }
    }
}

fn get_absolute_path(relative_to_executable: &str) -> Result<String, CliError> {
    let exe_path = try!(env::current_exe());
    let root_folder = exe_path.parent().expect("Filename should have a parent folder");

    let path = root_folder
        .join(Path::new(relative_to_executable))
        .to_str()
        .unwrap()
        .to_string();

    Ok(path)
}

fn parse_arguments() -> Result<Option<AppOptions>, CliError> {
    let default_data = "../../data/worldcitiespop.csv";
    let default_city = "Minsk";

    let args: Vec<String> = env::args().collect();
    let program = args[0].clone();

    let mut opts = Options::new();
    opts.optflag("h", "help", "Display this message");
    opts.optopt("d", "data", "Path to the data file", "<data-path>");
    opts.optopt("c", "city", "City", "<city>");
    opts.optflag("i", "stdin", "Read population data from stdin instead of a file");
    opts.optflag("q", "quiet", "Ignore errors and warnings");

    let matches = try!(opts.parse(&args[1..]));
    if matches.opt_present("h"){
        println!("{}", opts.short_usage(&program));
        Ok(None)
    } else {
        let use_stdin = matches.opt_present("i");
        let data = matches.opt_str("d");

        if use_stdin && data.is_some() {
            CliError::Args("When using stdin, filename shouldn't be specified".to_string());
        }

        let city = matches.opt_str("c").unwrap_or(default_city.to_string());
        let data =
            if use_stdin { None }
            else { Some(data.unwrap_or(try!(get_absolute_path(default_data)))) };

        Ok(Some(AppOptions::new(data, city, matches.opt_present("q"))))
    }
}

// TODO: return an iterator
fn find_population(data_source: &DataSource, city: &str) -> Result<Vec<Population>, CliError> {
    let input: Box<io::Read> = match *data_source {
        DataSource::File(ref p) => Box::new(try!(File::open(Path::new(p)))),
        DataSource::Stdin => Box::new(io::stdin())
    };

    let mut rdr = csv::Reader::from_reader(input);

    let populations: Vec<_> = rdr.decode::<Row>()
        .filter_map(|row| match row {
            Ok(r) =>
                if r.city == city && r.population.is_some() { Some(r) }
                else { None },
            _ => None
        })
        .map(|row| Population::new(row.country, row.city, row.population.unwrap()))
        .collect();

    if populations.is_empty() {
        Err(CliError::NotFound(city.to_string()))
    }
    else {
        Ok(populations)
    }

}

fn main() {
    let options = match parse_arguments(){
        Ok(Some(options)) => options,
        Err(error) => panic!(error.to_string()),
        _ => return,
    };

    println!("Cmd options {:?}", options);

    match find_population(&options.source, &options.city) {
        Err(CliError::NotFound(_)) if options.quiet => std::process::exit(1),
        Err(err) => panic!("{}", err),
        Ok(populations) => {
            for p in populations{
                println!("{:?}", p);
            }
        }
    }
}
