# RING Visual Novel Engine ~ And Revive The Melody ~

Ring Engine的AVG part核心组件

可靠、稳定、测试覆盖

# Architecture

## Core VN Runtime (Core)

核心运行时，独立维护在C#中的游戏状态和抽象功能操作

### RingIO

与主程序的交互界面，RAII管理全局资源以及process方法托管。

### Script

DSL 解析器，解释器，pythonnet搭建的inline code执行环境

- [x] 支持多文件
    - [x] changeScript
    - [ ] bonus:支持返回跳转点

### Stage

维护整个stage node下的subscene的状态，给main runtime提供一个简化的操作界面，本身不实现任何游戏逻辑。

#### Canvas

场景树Stage部分资源管理与类型转换。

### Animation

脚本的驱动部分

`IEffect`定义了异步执行单元，由C#的异步系统配合godot signal执行。

### Storage

|          |                    |
| -------- | ------------------ |
| 描述     | 运行状态存储、恢复 |
| 内部依赖 | 无                 |
| 外部依赖 | MessagePack        |

### UI

|          |                |
| -------- | -------------- |
| 描述     | 用户界面       |
| 内部依赖 | Core Animation |
| 外部依赖 | 无             |

### Audio

|          |                |
| -------- | -------------- |
| 描述     | 背景音乐、音效 |
| 内部依赖 | Core Animation |
| 外部依赖 | 无             |

### General

通用数据结构和工具方法

#### Logger/Assert

将log输出到godot控制台，运行时断言，失败可以log详细的错误位置并抛出异常。

#### PathSTD

统一的文件路径表示，转换godot和C#路径

#### UniformLoader

资源读写工具类

#### SceneTreeProxy

VNRuntime场景树对C#侧提供的静态界面，硬编码了场景结构，场景修改后要同步修改。

#### NodeExtensions

给各个Node类型添加的qol方法。



## TODO List

- [x] 并行动画实现改进（用于Show）



## Bug Fix List

- pythonnet的全局变量只会被创建一次，任何引用类型都不能假设对象是干净的，如果需要修改状态要clone一份。
