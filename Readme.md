# Shrew

[![Build](https://img.shields.io/appveyor/ci/phillyai/shrew/master.svg)](https://ci.appveyor.com/project/phillyai/shrew)
[![Tests](https://img.shields.io/appveyor/tests/phillyai/shrew/master.svg)](https://ci.appveyor.com/project/phillyai/shrew/build/tests)

모든 것이 매크로로 이루어진 엘릭서와 모든 것이 프로시저인 LISP 계열 언어처럼 모든 게 패턴인 언어

## 1차 목표

파라미터가 없이 단순 대입만 존재.

```c
a = 1
b = a + 3

main = a + b // main return 5
```

연산자는 괄호와 사칙연산 (+ - * /) 네 개를 지원하고 자료형은 정수, 실수, 부울 세 가지.
리터럴은 최소한만 생각함

커밋 [3222805](https://github.com/phillyai/shrew/commit/322280526522e7ef5ac79fbfd9c908dafe7b88cf) 에서 완성

## 2차 목표

빌트인 함수와 파라미터의 추가. 단 패턴 매칭은 안됨.

```c
add a b = a + b
main = add (add 1 2) 3
```

커밋 [133291e](https://github.com/phillyai/shrew/commit/133291ee904c6899481ead862dadba5b79232b0c) 에서 완성

## 3차 목표

문자열 (연산자 `++`로 concat), 단항 연산자 (`-`), 불리언/비트/그 외 연산자 (`! > < >= <= == != || && << >> ~ | & %`) 추가

연산자 우선순위

|순위|기호|설명|순서|
|--|--|--|--|
|1|-|단항|R2L|
|2|* / %||L2R|
|3|! ~ + - ++||L2R|
|4|<< >>|Bitwise shift|L2R|
|5|< > <= >= == !=||L2R|
|6|&|Bitwise AND|L2R|
|7|^|Bitwise XOR|L2R|
|8|\||Bitwise OR|L2R|
|9|&&|Logical AND|L2R|
|10|\|\||Logical OR|L2R|

## 4차 목표

패턴매칭 추가

```c
if true trueval _ = trueval
if false _ falseval = falseval_
main = if (3 > 1) (print "Bigger")
```

## 5차 목표

예외 처리 추가

**TODO**