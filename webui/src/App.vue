<template>
    <router-view/>
    <div id="application-background"></div>
</template>
<style>
html, body {
    z-index: -2;
    position: fixed;
    height: 100%;
    width: 100%;
    margin: 0;
    padding: 0;
    background-color: rgb(0,0,0);
}

#app {
    z-index: 0;
    position: fixed;
    height: 100%;
    width: 100%;
    top: 0;
    left: 0;
    background-color: rgb(0,0,0);
}

#application-background {
    z-index: -1;
    position: fixed;
    height: 100%;
    width: 100%;
    top: 0;
    left: 0;
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
    opacity: 0.3;
}
</style>
<script setup>
    import axios from "axios";

    axios({
        url: '/api/webui-support/background-image',
        method: 'get',
        responseType: "blob"
    }).then(res => {
        if (res.status === 200) {
            const url = URL.createObjectURL(res.data)
            document.querySelector('#application-background')
                .style.backgroundImage = `url('${url}')`
        }
    })
</script>