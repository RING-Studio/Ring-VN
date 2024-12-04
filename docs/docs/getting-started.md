# Getting Started

### 怎么写脚本

目前脚本支持的语句格式：

显示角色：

show <img src="assets/chara.png" alt="chara" style="zoom:25%;" /> as 名字 at 位置 with Effect

名字用来之后取回这张立绘（暂时没啥用），场上立绘的名字不能重复

位置： farleft、middle、nearright......

Effect：目前能用的就dissolve、fade，要改参数可以用内联代码`Dissolve(2.0)`，具体参数得看Effects.cs

> 图片要用HTML格式，不能用markdown格式（因为没做（），如`<img src="assets/bg1.png" alt="bg1" style="zoom:25%;" />`
>
> Typora里面右键图片可以切换格式

隐藏角色：

Hide 红叶 with Effect

切换场景：

changeScene <img src="assets/bg1.png" alt="bg1" style="zoom:25%;" /> with `ImageTrans()`

或者

changeBG <img src="assets/bg1.png" alt="bg1" style="zoom:25%;" /> with dissolve

上面一个可以带转场效果，下面一个只能带简单动画效果，之后会想办法合并这两个格式

转场效果目前只有`DissolveTrans()`和`ImageTrans()`，没有预定义对象，只能代码调用

说话：

红叶："114514"

支持中文冒号，目前台词显示速度不能调整，不支持空白人名

章节标题显示：

`# ChapterName`

跳转指令：

jump flag ? label1 : label2

其中flag是个全局变量，flag != 0时跳转label1，flag == 0跳转label2。

弹出选项卡：

```lua
```



所有内容都得写在根目录下面的main.md里，init.lua里的内容会在启动的时候执行，之后可以在代码块里调用。