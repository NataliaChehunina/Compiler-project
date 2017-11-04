# Compiler-project
Project work for course of theory of translators/compilers.

This program is a translator of pascal-like language to assembly code.

It works in 3 stages:

1) Parsing of source code (lexical analyze) and creating coded list of lexems and tables which contain codes of tokens.
Lexem (token) - elementary part of the programing language syntax. (such as reserved words, delimiters and identificators)

2) Parse tree creating. (syntax analyze)
Parse tree - special data structure which contains a program in form of tree according to rules list. 
Examples of rules:
"block" => "BEGIN", "statement list", "END"; (simple rule. node "block" with childrens)
"statement list" => "statement" , "statement list" |
                    "ENDLOOP" |
                    "END";
                    (recursive rule. may contain many "statement" nodes before encountering "ENDLOOP" node).
And so on.

3) Running through tree (depth first) and generation of assembly code.
