<template>
    <div>
        op1: {{ op1 }}
        <div v-if="loading">
            Loading...
        </div>
        <div v-if="users.length > 0">
            <div style="float:right">
                <button v-on:click.stop="newItem()">+ Add 702</button>
            </div>
            <table class="table table1" border="0">
                <thead>
                    <tr>
                        <th></th>
                        <th>Seq</th>
                        <th>Age</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th></th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(user, index) in users" v-bind:key="user.id">
                        <td>{{index+1}}</td>
                        <td>{{user.seq}}</td>
                        <td>
                            <div v-if="!user.edit">{{user.age}}</div>
                            <input class="form-control" v-if="user.edit" type="text" v-model.number="user.age" />
                        </td>
                        <td>{{user.FirstName}}</td>
                        <td>{{user.LastName}}</td>
                        <td>
                            <div v-if="!user.edit" style="min-width:40px; float:left;">
                                <a v-if="!user.edit" v-on:click.stop="user.edit=!user.edit">編輯</a>
                            </div>
                            <div v-if="user.edit" style="min-width:40px; float:left;">
                                <a v-on:click.stop="saveItem(user)">儲存</a>
                            </div>
                            <div style="min-width:40px; float:left;">
                                <a v-on:click.stop="delItem(index, user.seq)">刪除</a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>
    </div>
</template>
<script>
    export default {
        props: ['op1'],
        watch: {
            'op1': function (nval, oval) {
                console.log('watch op1 :' + oval + ' >> ' + nval);
                if (this.op1 > 0 ) { this.getList(); }
            }
        },
        data: function () {
            return {
                loading:false,
                users: [],
            };
        },
        methods: {
            async getList() {
                this.loading = true;
                const { data } = await window.myAjax.post('/QCStdBase/Chapter702', { op1: this.op1});
                this.users = data;
                this.loading = false;
            },
            newItem() {
                console.log('newItem()');
                var newRow = { seq: -1 * Date.now(), age:null, firstName:"", lastName:"", edit:true };
                this.users.push(newRow);
            },
            saveItem(item) {
                item.edit = false;
            },
            delItem(index, id) {
                //item.edit = false;
                console.log('index: ' + index + ' seq: ' + id);
                this.users.splice(index, 1);
            }
        },
        async mounted() {
            console.log('mounted() 第七章 702 職業安全衛生管理標準');
        }
    }
</script>