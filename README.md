# RING Visual Novel Engine ~ And Revive The Melody ~

Ring Engine的AVG part核心组件

可靠、稳定、测试覆盖

隔离引擎操作

## Architecture

### Core VN Runtime (Core)

核心运行时，独立维护在C#中的游戏状态和抽象功能操作

#### Script

|          |                       |
| -------- | --------------------- |
| 描述     | 脚本编译、运行        |
| 内部依赖 | Core其余组件          |
| 外部依赖 | Python3.11、pythonnet |
| 引擎依赖 | 无                    |

#### Stage

|          |                              |
| -------- | ---------------------------- |
| 描述     | 角色、背景等可视对象容纳     |
| 内部依赖 | Core Storage、Core Animation |
| 外部依赖 | 无                           |
| 引擎依赖 | EAL Canvas                   |

#### Animation

|          |                                        |
| -------- | -------------------------------------- |
| 描述     | 动画定义、执行                         |
| 内部依赖 | 无                                     |
| 外部依赖 | 无                                     |
| 引擎依赖 | EAL Tween、EAL其余系统（作为操作对象） |

#### Storage

|          |                    |
| -------- | ------------------ |
| 描述     | 运行状态存储、恢复 |
| 依赖     | 无                 |
| 外部依赖 | MessagePack        |
| 引擎依赖 | EAL Resource       |

#### UI

|          |                |
| -------- | -------------- |
| 描述     | 用户界面       |
| 依赖     | Core Animation |
| 外部依赖 | 无             |
| 引擎依赖 | EAL Canvas     |

#### Audio

|          |                |
| -------- | -------------- |
| 描述     | 背景音乐、音效 |
| 依赖     | Core Animation |
| 外部依赖 | 无             |
| 引擎依赖 | EAL Audio      |

### Engine Abstraction Layer (EAL)

引擎资源抽象，负责所有与godot对象的操作

#### Canvas

场景树Stage部分资源管理与类型转换。

#### Tween

基础动画效果实现。

#### Resource

资源素材抽象

导出：

UniformLoader：统一资源加载接口，提供各种静态加载方法。

Renderable：Sprite2D包装层，隔离godot类型以及提供qol方法。

AudioPlayer：AudioStreamPlayer包装层，隔离godot类型以及提供qol方法。

#### SceneTreeProxy

场景树代理静态类，维护场景树结构，提供godot类型和ring类型的转换与隔离。

#### General

通用模块

导出：

Logger：标准日志记录接口，输出到godot console方便调试。

AssetWrapper：静态类，提供断言方法集合，有丰富的debug信息。

PathSTD：标准文件系统路径，统一Godot和C#的文件路径。

#### Global

一个Autoload节点，处理一些全局资源的管理和process方法的托管。
