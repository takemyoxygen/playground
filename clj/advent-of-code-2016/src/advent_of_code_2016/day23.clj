(ns advent-of-code-2016.day23
  (:require [advent-of-code-2016.monorail :as monorail]))

(defn get-initial-state
  [instructions]
  {:next 0 :instructions (vec instructions) :registers {}})

(defn resolve-value
  [operand state]
  (if (= (:type operand) :constant)
    (:value operand)
    (get-in state [:registers (:name operand)] 0)))

(defn move-next
  [state]
  (update state :next inc))

(defn toggle-instruction
  [{:keys [name operands] :as instruction}]
  (cond
    (and (= 1 (count operands)) (= "inc" name)) {:name "dec" :operands operands}
    (= 1 (count operands)) {:name "inc" :operands operands}
    (and (= 2 (count operands)) (= "jnz" name)) {:name "cpy" :operands operands}
    (= 2 (count operands)) {:name "jnz" :operands operands}
    :else instruction))

(defn set-register
  [register value state]
  (assoc-in state [:registers (:name register)] value))

(defn update-register
  [register f state]
  (let [current (resolve-value register state)]
    (set-register register (f current) state)))

(defn perform-tgl
  [{[operand] :operands} {:keys [instructions] :as state}]
  (let [instruction-index (+ (:next state) (resolve-value operand state))
        new-instructions (if
                           (contains? instructions instruction-index)
                           (update instructions instruction-index toggle-instruction)
                           instructions)]
    (-> state
        (assoc :instructions new-instructions)
        (move-next))))

(defn perform-cpy
  [{[op1 op2] :operands} state]
  (if
    (monorail/register? op2)
    (->> state
         (set-register op2 (resolve-value op1 state))
         move-next)
    (move-next state)))

(defn perform-inc
  [{[op] :operands} state]
  (if
    (monorail/register? op)
    (->> state
         (update-register op inc)
         move-next)
    (move-next state)))

(defn perform-dec
  [{[op] :operands} state]
  (if
    (monorail/register? op)
    (->> state
         (update-register op dec)
         move-next)
    (move-next state)))

(defn perform-mult
  [{[op1 op2] :operands} state]
  (let [result (* (resolve-value op1 state) (resolve-value op2 state))]
    (->> state
         (set-register op1 result)
         (move-next))))

(defn perform-jnz
  [{[op steps] :operands} state]
  (let [steps-value (resolve-value steps state)
        registry-value (resolve-value op state)]
    (if (= registry-value 0)
      (move-next state)
      (update state :next (partial + steps-value)))))

(defn perform-operation
  [operation state]
  (case (:name operation)
    "cpy" (perform-cpy operation state)
    "inc" (perform-inc operation state)
    "dec" (perform-dec operation state)
    "jnz" (perform-jnz operation state)
    "tgl" (perform-tgl operation state)
    "mult" (perform-mult operation state)
    (throw (Exception. (str "Unknown operation" operation)))))

(defn execute-instructions
  [state]
  (loop [{:keys [instructions next] :as current-state} state]
    (if (contains? instructions next)
      (recur (perform-operation (instructions next) current-state))
      current-state)))


; several instructions in the original input are replaced with new "mult" instruction followed
; by setting a few registers to 0 and then jumping to the end of the multiplication (to keep the total number of instructions)
; the same as in the original input
(def input "cpy a b\ndec b\nmult a b\ncpy 0 c\ncpy 0 d\njnz 1 5\ndec c\njnz c -2\ndec d\njnz d -5\ndec b\ncpy b c\ncpy c d\ndec d\ninc c\njnz d -2\ntgl c\ncpy -16 c\njnz 1 c\ncpy 73 c\njnz 82 d\ninc a\ninc d\njnz d -2\ninc c\njnz c -5")

; Part 1
(->> input
     (monorail/parse-instructions)
     (get-initial-state)
     (set-register {:name "a"} 7)
     (execute-instructions)
     (:registers)
     (println "Part 1"))

; Part 2
(->> input
     (monorail/parse-instructions)
     (get-initial-state)
     (set-register {:name "a"} 12)
     (execute-instructions)
     (:registers)
     (println "Part 2"))
