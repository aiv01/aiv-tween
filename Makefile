all:
	mcs -sdk:2 -target:library -out:aiv-tween.dll src/Tween.cs src/Easing.cs
