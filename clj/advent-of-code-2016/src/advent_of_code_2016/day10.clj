(ns advent-of-code-2016.day10)

(def input "value 5 goes to bot 2\nbot 2 gives low to bot 1 and high to bot 0\nvalue 3 goes to bot 1\nbot 1 gives low to output 1 and high to bot 0\nbot 0 gives low to output 2 and high to output 0\nvalue 2 goes to bot 2")

(defn parse-line
  [line]
  (if-let [[_ val bot] (re-matches #"value (\d+) goes to bot (\d+)" line)]
    {:value (Integer/parseInt val) :bot (Integer/parseInt bot)}
    (if-let [[_ bot low-type low high-type high] (re-matches #"bot (\d+) gives low to (bot|output) (\d+) and high to (bot|output) (\d+)" line)]
      {:bot (Integer/parseInt bot) :low-type (keyword low-type) :low (Integer/parseInt low) :high-type (keyword high-type) :high (Integer/parseInt high)}
      (throw (Exception. (str "Invalid input: " line))))))

(defn process-instruction
  [state instruction]
  (if-let [val (:value instruction)]
    (merge-with clojure.set/union state {(:bot instruction) #{val}})
    state))

(defn process-instructions
  [instructions]
  (reduce process-instruction {} instructions))

(defn parse-input
  [input]
  (->> input
       (clojure.string/split-lines)
       (map parse-line)))