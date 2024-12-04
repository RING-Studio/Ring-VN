# RE: Ring Engine

## BUG

- [x] 跨过选择支后历史记录会消失（feature or bug？）

    ​	在snapshot里面添加history解决，history可以在内存暂存但不会保存到磁盘上

    - [ ] 跨选择支回退的时候倒回选择支前的时候需要多按一下回退（有一个多余history），做backlog的时候一并解决

- [x] 从小游戏切换回AVG会冻结游戏

    ​	已修复，小游戏把树暂停导致的（

- [x] BG不在名为BG的节点上

    ​	已修复，看上去是merge错误

- [ ] 动画未结束时进行backlog会使动画系统崩溃

    ​	最近没碰到了，等复现了再说

## TODO List

**new:**

- [ ] 重要重构：分离Game Runtime和AVL Runtime
    - [x] Game Runtime包含存档、全局设置，AVL Runtime包含剩余的Runtime内容
    - [x] 创建Game Runtime下切换子Runtime功能
    - [x] 以附加Runtime形式实现选项卡
    - [x] 以附加Runtime形式接入小游戏
    - [ ] 以附加Runtime形式实现Gallary
    - [x] 以附加Runtime形式实现Backlog
        - [ ] 高级功能：鼠标悬浮时显示对应的场景
    
- [ ] 测试release build到没有python的机器上会不会有PYTHONPATH问题

- [x] 弹出选项卡

    选项卡是一个场景，调用的时候AVG scene应该处于paused状态

    - [ ] pause会不会影响UI响应，选择支应当允许保存
        - [ ] 可以在选择支隐去UI来解决（

​	输入为构造参数：每个选项显示内容、是否可以选中（去音乐会.jpg）

​	输出为选中的选项编号

   - [x] 竖排选项卡

- [ ] 横排选项卡

- [x] 选项卡的native脚本格式：表格

    | 竖排/横排/Vertical/Horizontal |
    | ----------------------------- |
    | 选项1                         |
    | 选项2                         |
    | 选项3                         |

    | 竖排/横排/Vertical/Horizontal |        |
    | ----------------------------- | ------ |
    | 选项1                         | label1 |
    | 选项2                         | label2 |
    | 选项3                         | label3 |

- [x] Parser需要支持跨行结构解析

- [ ] 数值面板

- [x] ParserException包含行数

- [x] show同名立绘实现crossfade

- [x] 动效顺序debug

- [ ] 增添BG Placement

- [ ] 实现Camera移动动画

- [ ] 导出脚本块@continue属性



- [ ] 如何对依赖godot runtime的代码进行单元测试
    - [ ] https://gitlab.com/jfletcher94/gd-net-scout/

- [x] 脚本语言选择：python/pythonnet
  - [ ] 如何提供intellisense支持
  
- [ ] 跳转语句如何设计？
    - [ ] jump to file start（无难度）（叫什么名字，goto已经给label jump了）
    - [x] jump to given label
        - [x] 设置label
        - [x] label作用域：文件

    - [x] 选择支语句设计

- [ ] 实现常用的蒙版、Camera、shader动画
- [ ] UI模块
- [x] <b>Refactor：从UI和Canvas中抽离脚本逻辑到ScriptBlock，动画状态追踪由EffectBuffer负责</b>

脚本相关：

- [x] 添加专门的.lua初始化配置文件

- [x] 将Placement配置迁移到lua侧动态调整

动画相关：

- [x] 实现动画缓冲队列

​	   多语句块同时提交时队列按序存放每个语句所用的Effects，首先为语句1分配Tween，监听Tween finish信号，语句1执行完毕（正常/快进）后给语句2分配Tween并执行......

​	   应该可以彻底解决动画相关的并发问题

动画重构，chain effect bug —— group间不存在这个问题（后一组在前一组执行完成后才提交，获取的参数值是最新的），group内Effect默认使用前一个Effect的末值作为初值（不指定初值），单独添加方法来立即修改属性值。

- [x] 快进的时候chapter name的动画效果如何处理 —— 使用EffectBuffer可以在单语句块中定义多段快进动画，另外添加nonBlockingBuffer处理和其它效果并行的效果
- [ ] 实现多节点效果（Transition）
    - [ ] 基于shader的蒙版动画（还需要添加reverse参数和调整缓动）
    - [ ] 需要的接口暴露给lua运行时


素材格式约定：

给定默认图像分辨率：（待确定）

垂直位置确定：对每个角色添加水平基线，显示时所有基线y值相同

- [ ] 基线值如何给出：
    - 与角色名绑定，用一个json记录每个人的baseline offset，根据立绘索引名称确定是谁的立绘

- [ ] 假设所有角色图像以该分辨率给出且水平居中，根据分辨率计算各个位置的scale和offset



- [x] 存档系统：
    - [x] 组件序列化/反序列化：全局变量，Canvas，UI，维持Script和LuaInterpreter无状态
    - [x] 存档目录结构
    - [x] 内存暂存快照（History）
    
    能做到的操作：单步回退、任意步回退、指定位置保存、~~重载后恢复历史~~
    
    Canvas.scn体积大的原因是素材在挂上去的时候做了resize，这一步重新生成了一个新的Image不对应磁盘上的任何文件，所以保存的时候需要存储，这个问题的原因是当前素材分辨率不一致，需要载入时resize，统一分辨率即可。(Edit：已完成，素材在初次载入时会被缩放并修改原文件)
    
- [ ] avatar support

## Tips

- C#脚本抛出异常~~不会传递到Godot侧~~会传过来，但是在调试器里，只有整个.NET运行时崩溃才会在输出里报错
