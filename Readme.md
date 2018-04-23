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

문자열 (연산자 `++`로 concat), 단항 연산자 (`+ -`), 불리언/비트/그 외 연산자 (`! > < >= <= == != || && << >> ~ | & %`) 추가

연산자 우선순위

|순위|기호|설명|순서|
|--|--|--|--|
|1|! ~ -|단항|R2L|
|2|* / %||L2R|
|3|+ - ++||L2R|
|4|<< >>|Bitwise shift|L2R|
|5|< > <= >= == !=||L2R|
|6|&|Bitwise AND|L2R|
|7|^|Bitwise XOR|L2R|
|8|\||Bitwise OR|L2R|
|9|&&|Logical AND|L2R|
|10|\|\||Logical OR|L2R|
|11|=||R2L|

커밋 [215ddda](https://github.com/phillyai/shrew/commit/215dddad79e0ca82898f70932be14c8ba3df2028)에서 완성

## 4차 목표

패턴매칭 추가

```c
if true trueval _ = trueval
if false _ falseval = falseval
main = print (if (3 > 1) "Bigger" "Smaller")
```
```c
>>> sum_iter 0 res = res
>>> sum_iter n res = sum_iter (n-1) (res+n)
>>> sum n = sum_iter n 0
>>> sum 100
5050
```

2차 목표때 해결했어야 할 문젠데 패턴의 파라미터의 타입을 모르니까 **일단 파라미터로 받는 모든 값은 파라미터를 받지 않는 타입으로 가정함**. (이슈 [#2](https://github.com/phillyai/shrew/issues/3) 참고)


커밋 [36759ec](https://github.com/phillyai/shrew/commit/36759ec2c6ed8000136a83bc62d40890963e0a72)에서 완성

## 5차 목표

파라미터로 인자 1개이상 받는 타입 보낼 수 있게 함.
대신 문장의 끝을 개행문자로 구분하도록 함.

그럼 기존의 테스트 케이스 몇 개를 수정해야 함

```c
a 0 b = b a 1 b = b - 1
```

원래라면 `a 0 b = b` 와 `a 1 b = b - 1` 으로 `b`가 파라미터 받지 않는 타입으로 생각해서 자동으로 끊어서 파싱했을텐데 더이상 그러면 안됨 ㅎ
개행문자를 만나기 전까지는 한 statement로 생각하고 파싱을 시도하도록 함
그래서 아마 `a 0 b = b a 1 b` 까지 생각하고 `=` 을 만나는 순간 파싱 에러를 일으켜야함

암튼 그래서 고차함수가 됨 ㅎ

```c
twice f x = f (f x)
p3 x = x * 2
main = twice p3 10 // 40
```

**TODO**
