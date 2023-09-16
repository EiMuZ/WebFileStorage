import {createRouter, createWebHistory} from 'vue-router'
import SignInView from "@/view/SignInView.vue";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/signin',
            component: SignInView
        }
    ]
})

router.beforeEach((to, from, next)=>{
    next()
})

export default router