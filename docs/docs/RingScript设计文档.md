# RingScript设计文档

Parser：基于正则表达式的文档语法解析和基于三方运行时的代码解析

Runtime：AST traverse，由于文档语法没有递归结构，AST实际为List，代码由三方运行时执行

Environment：项目其他部分提供运行时环境，并导入三方运行时

