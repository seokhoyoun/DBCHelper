# DBCHelper (Current Ver. 0.1V Beta)
Load/Parse .dbc file and provides some function for signal input/output

## Project List
- DBCHelper : Console program for load .dbc files
- CANCommunication : DBCHelper's Wpf program 
- DBCHelperWpf : demo UI

## Supported Functions 
- Nodes
- Messages
- Signals
- Value Tables
- Environment Variables
- Attributes

## Not Supported Functions
- Attribute Relative(?) 
- Attribute Definition Relative(?)
- Multiplexing

```
// 지원하지 않는 기능 항목은 아래와 같이 Assert 처리 되어있음
// 일반적으로 사용되지않거나 더 이상 쓰이지 않음(deprecated) 또는 해당 속성에 대한 이해 부족으로인해 미작성 됨
// 필요에 따라 직접 구현하여 쓸 것.

if (rawLine.StartsWith(ATTRIBUTE_RELATIVE_IDENTIFIER, COMPARISON) || rawLine.StartsWith(ATTRIBUTE_DEF_REL_IDENTIFIER, COMPARISON))
{
    Debug.Assert(false);
    continue;
}
```

## TODO List (2020/12/17 updated)
```
* 통신 부 구현하여, 실제 데이터 Bit Converter 클래스 이용하여, UI에 뿌리기
* 초기 값, Cycle time 등의 Attribute 할당
* 설정 단 저장 기능 추가 필요
```

