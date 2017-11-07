import Data.List (stripPrefix)
import Data.List.Split (splitOn)
import Data.Maybe (mapMaybe)

data Instruction 
    = Rect Int Int
    | RotateRow Int Int
    | RotateColumn Int Int
    deriving (Show)

parseRect :: String -> Maybe Instruction
parseRect input =
    (\[x, y] -> Rect x y) 
    <$> (map read . splitOn "x")
    <$> (stripPrefix "rect " input)

parseRotate :: String -> Maybe Instruction
parseRotate input =
    case stripPrefix "rotate " input of
    Just left -> 
        case splitOn " " left of
        ["column", ('x':'=':coord), "by", step] -> Just $ RotateColumn (read coord) (read step)
        ["row", ('y':'=':coord), "by", step] -> Just $ RotateRow (read coord) (read step)
        _ -> Nothing
    _ -> Nothing

parse :: String -> Instruction
parse line =
    let parsers = [parseRect, parseRotate]
    in head $ mapMaybe (\parser -> parser line) parsers

main :: IO ()
main = do
    input <- lines <$> getContents
    let instructions = map parse input
    mapM_ print instructions
    print $ length instructions