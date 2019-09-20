(ns advent-of-code-2016.monorail)

(defn- parse-operand
  [operand]
  (if (nil? operand)
    nil
    (try
      {:type :constant :value (Integer/parseInt operand)}
      (catch NumberFormatException _
        {:type :register :name operand}))))

(defn- parse-line
  [input-line]
  (let [[operation operand1 operand2] (clojure.string/split input-line #"\s")]
    {:name operation :operands (remove nil? [(parse-operand operand1) (parse-operand operand2)])}))

(defn parse-instructions
  "Parses a string containing a set of instructions into a sequence of instruction maps"
  [input]
  (map parse-line (clojure.string/split-lines input)))

(defn register?
  [{:keys [type]}]
  (= type :register))

(defn constant?
  [{:keys [type]}]
  (= type :constant))
