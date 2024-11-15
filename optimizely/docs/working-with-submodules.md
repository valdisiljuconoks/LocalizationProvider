#WIP

* adding submodule
`git submodule add ... lib/...`

`git submodule foreach --recursive git checkout master`

* after cloning (to checkout also submodules):
`git submodule update --init --recursive`

* for the submodules:
```
<Reference>
	<HintPath>$(SolutionDir)\
</Reference>
```

* updating to the latest version (last commit) of the submodule
`git submodule foreach git pull origin master`

