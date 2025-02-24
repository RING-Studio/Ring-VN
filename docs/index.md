---
_layout: landing
---

# This is the **HOMEPAGE**.

## Recent Updates

- 添加了RingScript native选择支语句，使用表格

    - 单列表格仅在`LastChosenOptionId`中记录选项，需要配合Code工作

        | 竖排/横排/Vertical/Horizontal |
        | ----------------------------- |
        | 选项1                         |
        | 选项2                         |

    - 双列表格在选择后自动跳转到对应的label

        | 竖排  |        |
        | ----- | ------ |
        | 选项1 | label1 |
        | 选项2 | label2 |

    - 暂时不支持横排选项卡

- 添加了竖排选项卡，可以在AVGRuntime中通过CodeBlock调用Branch接口，选择储存在全局变量`LastChosenOptionId`中。

- 正在实现用户设置语句后是否继续执行的支持，目前在CodeBlock中先实现，将识别脚本语言的identifier设成false可以阻止自动继续执行，用于SubRuntime切换时防止脚本继续执行。

- 内置代码支持切换到了Python，解决了大量Nlua bug带来的问题，现在内置脚本可以和C#脚本一样访问引擎的所有功能，并且有良好的互操作性。

- 添加了存档支持，可以按Esc存档，存档位于snapshot文件夹下，开始游戏时按加载游戏可载入存档。

- 添加了Backlog支持，按Backspace回退一步，仅支持当前运行中看过的部分。

- 添加了enable Feature/disable Feature的语句，目前用于强制继续执行（连续动画）

- 蒙版动画现在可以直接在changeScene的with效果中拖拽图片上去