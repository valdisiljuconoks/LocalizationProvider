# vue-directive-tooltip

![Node](https://img.shields.io/node/v/vue-directive-tooltip.svg?style=flat-square)
[![NPM](https://img.shields.io/npm/v/vue-directive-tooltip.svg?style=flat-square)](https://www.npmjs.com/package/vue-directive-tooltip)
[![NPM Downloads/month](https://img.shields.io/npm/dm/vue-directive-tooltip.svg?style=flat-square)](https://www.npmjs.com/package/vue-directive-tooltip)
[![Vue.js](https://img.shields.io/badge/vue-2-green.svg)](https://vuejs.org)

> Vue.js tooltip directive ([based on Popper.js](https://github.com/FezVrasta/popper.js))


### Installation

`yarn add vue-directive-tooltip`

or

`npm install vue-directive-tooltip --save`


### Usage

Super simple

```html
<span v-tooltip="'my text'">some text</span>
```

A **lot of options** make this tooltip convenient:

- **content**: simple text or use another html tag
- **positioning** (right, left, ...)
- **positioning variants**: start | end. ex: bottom.end, top.start, right.end, etc...
- **delay** to hide (in ms)
- **offset** (from the target in px)
- choose from **several triggers** (hover, focus, click)
- **full programmatic**, you choose when to show/hide the tooltip
- and of course, use your **custom CSS** class


### Documentation

[Full documentation with examples](https://hekigan.github.io/vue-directive-tooltip/)

[<img src="https://hekigan.github.io/vue-directive-tooltip/img/screenshot.jpg">](https://hekigan.github.io/vue-directive-tooltip/)

### Builds

If you don't use a package manager, you can [access `vue-directive-tooltip` via unpkg (CDN)](https://unpkg.com/vue-directive-tooltip/), download the source, or point your package manager to the url.

`vue-directive-tooltip` is compiled as a collection of [CommonJS](http://webpack.github.io/docs/commonjs.html) modules & [ES2015 modules](http://www.2ality.com/2014/09/es6-modules-final.html) for bundlers that support the `jsnext:main` or `module` field in package.json (Rollup, Webpack 2)

The `vue-directive-tooltip` package includes precompiled production and development [UMD](https://github.com/umdjs/umd) builds in the [`dist` folder](https://unpkg.com/vue-directive-tooltip/dist/). They can be used directly without a bundler and are thus compatible with many popular JavaScript module loaders and environments. You can drop a UMD build as a [`<script>` tag](https://unpkg.com/vue-directive-tooltip) on your page. The UMD builds make `vue-directive-tooltip` available as a `window.vueDirectiveTooltip` global variable.

### License

The code is available under the [MIT](LICENSE) license.

### Contributing

We are open to contributions, see [CONTRIBUTING.md](CONTRIBUTING.md) for more info.

### Misc

This module was created using [generator-module-extended-boilerplate](https://github.com/hekigan/generator-module-extended-boilerplate).
