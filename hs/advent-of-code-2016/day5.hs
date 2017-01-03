import Data.List (isPrefixOf, find, sortOn)
import Data.ByteString.Char8 (pack)
import Crypto.Hash
import Data.HashMap.Strict (HashMap, insertWith, empty, toList)
import Data.Char (isDigit, digitToInt)

-- I guess this one can be significantly refactored and optimized

md5 :: String -> Digest MD5
md5 = hash . pack

handlePassword :: (HashMap Int Char, String) -> String -> (HashMap Int Char, String)
handlePassword (positioned, code) password =
    let indexChar:rest = drop 5 $ password
        positioned2 = 
            if (not $ null rest ) && isDigit indexChar && indexChar < '8' 
            then insertWith (\new old -> old) (digitToInt indexChar) (head rest) positioned 
            else positioned
        code2 = if length code < 8 then indexChar : code else code
    in (positioned2, code2)

solve :: String  -> (String, String)
solve prefix =
    let combine n = prefix ++ show n
        passwords = filter (isPrefixOf "00000") $ map (show . md5 . combine) $ iterate (+1) 0
        combined = drop 1 $ scanl handlePassword (empty, []) passwords
        (positioned, code) = head $ filter (\(positioned, _) -> length positioned == 8) combined
    in (reverse code, map snd $ sortOn fst $ toList positioned)

main :: IO ()
main = do
    putStrLn "Enter Door ID:"
    input <- getLine
    print $ solve input
